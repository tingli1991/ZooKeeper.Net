﻿using log4net;
using Org.Apache.Jute;
using Org.Apache.Zookeeper.Proto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZooKeeper.Net.IO;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientConnectionRequestProducer : IStartable, IDisposable
    {
        private readonly ClientConnection conn;
        private readonly ZooKeeper zooKeeper;
        private readonly Thread requestThread;

        private readonly ConcurrentQueue<Packet> pendingQueue = new ConcurrentQueue<Packet>();
        private readonly LinkedList<Packet> outgoingQueue = new LinkedList<Packet>();
        private readonly AutoResetEvent packetAre = new AutoResetEvent(false);
        private const string RETRY_CONN_MSG = ", closing socket connection and attempting reconnect";
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientConnectionRequestProducer));

        /// <summary>
        /// 
        /// </summary>
        public int PendingQueueCount
        {
            get
            {
                return pendingQueue.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int OutgoingQueueCount { get { return outgoingQueue.Count; } }
        private TcpClient client;
        private readonly Random random = new Random();
        private int initialized;
        internal long lastZxid;
        private long lastPingSentNs;
        internal int xid = 1;
        private volatile bool closing;
        private byte[] incomingBuffer = new byte[4];
        internal int sentCount;
        internal int recvCount;
        internal int negotiatedSessionTimeout;
        private ZooKeeperEndpoints zkEndpoints;
        private int connectionClosed;

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnectionClosedByServer
        {
            get
            {
                return Interlocked.CompareExchange(ref connectionClosed, 0, 0) == 1;
            }
            private set
            {
                Interlocked.Exchange(ref connectionClosed, value ? 1 : 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        public ClientConnectionRequestProducer(ClientConnection conn)
        {
            this.conn = conn;
            zooKeeper = conn.zooKeeper;
            zkEndpoints = new ZooKeeperEndpoints(conn.serverAddrs);
            requestThread = new Thread(new SafeThreadStart(SendRequests).Run) { Name = new StringBuilder("ZK-SendThread ").Append(conn.zooKeeper.Id).ToString(), IsBackground = true };
        }

        /// <summary>
        /// 
        /// </summary>
        protected int Xid
        {
            get { return xid++; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            requestThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h"></param>
        /// <param name="r"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="clientPath"></param>
        /// <param name="serverPath"></param>
        /// <param name="watchRegistration"></param>
        /// <returns></returns>
        public Packet QueuePacket(RequestHeader h, ReplyHeader r, IRecord request, IRecord response, string clientPath, string serverPath, WatchRegistration watchRegistration)
        {
            if (h.Type != (int)OpCode.Ping && h.Type != (int)OpCode.Auth)
                h.Xid = Xid;

            Packet p = new Packet(h, r, request, response, null, watchRegistration, clientPath, serverPath);
            if (!zooKeeper.State.IsAlive() || closing || Interlocked.CompareExchange(ref isDisposed, 0, 0) == 1)
            {
                if (log.IsDebugEnabled)
                    log.DebugFormat("Connection closing. Sending ConLossPacket. IsAlive: {0}, closing: {1}", zooKeeper.State.IsAlive(), closing);
                ConLossPacket(p);
            }
            else
            {
                if (h.Type == (int)OpCode.CloseSession)
                    closing = true;

                addPacketLast(p);
            }
            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        private void addPacketFirst(Packet p)
        {
            outgoingQueue.AddFirst(p);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        private void addPacketLast(Packet p)
        {
            lock (outgoingQueue)
            {
                outgoingQueue.AddLast(p);
            }
            packetAre.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendRequests()
        {
            DateTime now = DateTime.UtcNow;
            DateTime lastSend = now;
            Packet packet = null;

            while (zooKeeper.State.IsAlive())
            {
                try
                {
                    now = DateTime.UtcNow;
                    if ((client == null || client.Client == null) || (!client.Connected || zooKeeper.State == States.NOT_CONNECTED))
                    {
                        // don't re-establish connection if we are closing
                        if (conn.IsClosed || closing)
                            break;

                        StartConnect();
                        lastSend = now;
                    }
                    TimeSpan idleSend = now - lastSend;
                    if (zooKeeper.State == States.CONNECTED)
                    {
                        TimeSpan timeToNextPing = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(conn.readTimeout.TotalMilliseconds / 2 - idleSend.TotalMilliseconds));
                        if (timeToNextPing <= TimeSpan.Zero)
                            SendPing();
                    }
                    // Everything below and until we get back to the select is
                    // non blocking, so time is effectively a constant. That is
                    // Why we just have to do this once, here                    

                    packet = null;
                    lock (outgoingQueue)
                    {
                        if (!outgoingQueue.IsEmpty())
                        {
                            packet = outgoingQueue.First();
                            outgoingQueue.RemoveFirst();
                        }
                    }
                    if (packet != null)
                    {
                        // We have something to send so it's the same
                        // as if we do the send now.                        
                        DoSend(packet);
                        lastSend = DateTime.UtcNow;
                        packet = null;
                    }
                    else
                        packetAre.WaitOne(TimeSpan.FromMilliseconds(1));
                }
                catch (Exception e)
                {
                    if (conn.IsClosed || closing)
                    {
                        if (log.IsDebugEnabled)
                        {
                            // closing so this is expected
                            log.DebugFormat("An exception was thrown while closing send thread for session 0x{0:X} : {1}", conn.SessionId, e.Message);
                        }
                        break;
                    }
                    else
                    {
                        // this is ugly, you have a better way speak up
                        if (e is KeeperException.SessionExpiredException)
                            log.InfoFormat("{0}, closing socket connection", e.Message);
                        else if (e is SessionTimeoutException)
                            log.InfoFormat("{0}{1}", e.Message, RETRY_CONN_MSG);
                        else if (e is System.IO.EndOfStreamException)
                            log.InfoFormat("{0}{1}", e.Message, RETRY_CONN_MSG);
                        else
                            log.InfoFormat("Session 0x{0:X} for server {1}, unexpected error{2}, detail:{3}-{4}", conn.SessionId, null, RETRY_CONN_MSG, e.Message, e.StackTrace);
                        // a safe-net ...there's a packet about to send when an exception happen
                        if (packet != null)
                            ConLossPacket(packet);
                        // clean up any queued packet
                        Cleanup();
                        if (zooKeeper.State.IsAlive())
                        {
                            conn.consumer.QueueEvent(new WatchedEvent(KeeperState.Disconnected, EventType.None, null));
                        }
                    }
                }
            }

            // safe-net to ensure everything is clean up properly
            Cleanup();

            // i don't think this is necessary, when we reached this block ....the state is surely not alive
            if (zooKeeper.State.IsAlive())
                conn.consumer.QueueEvent(new WatchedEvent(KeeperState.Disconnected, EventType.None, null));

            if (log.IsDebugEnabled)
                log.Debug("SendThread exitedloop.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tcpClient"></param>
        private void Cleanup(TcpClient tcpClient)
        {
            if (tcpClient != null)
            {
                try
                {
                    // close the connection
                    tcpClient.Close();
                }
                catch (IOException e)
                {
                    if (log.IsDebugEnabled)
                        log.Debug("Ignoring exception during channel close", e);
                }
            }

            lock (outgoingQueue)
            {
                foreach (var packet in outgoingQueue)
                {
                    ConLossPacket(packet);
                }
                outgoingQueue.Clear();
            }

            Packet pack;
            while (pendingQueue.TryDequeue(out pack))
                ConLossPacket(pack);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Cleanup()
        {
            Cleanup(client);
            client = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartConnect()
        {
            zooKeeper.State = States.CONNECTING;
            TcpClient tempClient = null;
            WaitHandle wh = null;
            do
            {
                if (zkEndpoints.EndPointID != -1)
                {
                    try
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 0, 0, random.Next(0, 50)));
                    }
                    catch (ThreadInterruptedException e1)
                    {
                        log.Warn("Unexpected exception", e1);
                    }
                    if (!zkEndpoints.IsNextEndPointAvailable)
                    {
                        try
                        {
                            // Try not to spin too fast!
                            Thread.Sleep(1000);
                        }
                        catch (ThreadInterruptedException e)
                        {
                            log.Warn("Unexpected exception", e);
                        }
                    }
                }

                //advance through available connections;
                zkEndpoints.GetNextAvailableEndpoint();

                Cleanup(tempClient);
                log.InfoFormat("Opening socket connection to server {0}", zkEndpoints.CurrentEndPoint.ServerAddress);
                tempClient = new TcpClient();
                tempClient.LingerState = new LingerOption(false, 0);
                tempClient.NoDelay = true;

                Interlocked.Exchange(ref initialized, 0);
                IsConnectionClosedByServer = false;

                try
                {
                    IAsyncResult ar = tempClient.BeginConnect(zkEndpoints.CurrentEndPoint.ServerAddress.Address,
                                                          zkEndpoints.CurrentEndPoint.ServerAddress.Port,
                                                          null,
                                                          null);

                    wh = ar.AsyncWaitHandle;
                    if (!ar.AsyncWaitHandle.WaitOne(conn.ConnectionTimeout, false))
                    {
                        Cleanup(tempClient);
                        tempClient = null;
                        throw new TimeoutException();
                    }

                    tempClient.EndConnect(ar);

                    break;
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is TimeoutException)
                    {
                        Cleanup(tempClient);
                        tempClient = null;
                        zkEndpoints.CurrentEndPoint.SetAsFailure();

                        log.WarnFormat(string.Format("Failed to connect to {0}:{1}.",
                            zkEndpoints.CurrentEndPoint.ServerAddress.Address.ToString(),
                            zkEndpoints.CurrentEndPoint.ServerAddress.Port.ToString()));
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    wh.Close();
                }
            }
            while (zkEndpoints.IsNextEndPointAvailable);

            if (tempClient == null)
            {
                throw KeeperException.Create(KeeperException.Code.CONNECTIONLOSS);
            }

            client = tempClient;
            client.GetStream().BeginRead(incomingBuffer, 0, incomingBuffer.Length, ReceiveAsynch, incomingBuffer);
            PrimeConnection();
        }

        private byte[] juteBuffer;
        private int currentLen;

        /// <summary>
        /// process the receiving mechanism in asynchronous manner.
        /// Zookeeper server sent data in two main parts
        /// part(1) -> contain the length of the part(2)
        /// part(2) -> contain the interest information
        /// 
        /// Part(2) may deliver in two or more segments so it is important to 
        /// handle this situation properly
        /// </summary>
        /// <param name="ar">The asynchronous result</param>
        private void ReceiveAsynch(IAsyncResult ar)
        {
            if (Interlocked.CompareExchange(ref isDisposed, 0, 0) == 1)
            {
                return;
            }
            int len = 0;
            try
            {
                if (client == null)
                    return;
                NetworkStream stream = client.GetStream();

                try
                {
                    len = stream.EndRead(ar);
                }
                catch
                {
                }
                if (len == 0) //server closed the connection...
                {
                    log.Debug("TcpClient connection lost.");
                    if (zooKeeper.State == States.CONNECTING)
                        zkEndpoints.CurrentEndPoint.SetAsFailure();

                    zooKeeper.State = States.NOT_CONNECTED;
                    IsConnectionClosedByServer = true;
                    return;
                }
                byte[] bData = (byte[])ar.AsyncState;
                recvCount++;

                if (bData == incomingBuffer) // if bData is incoming then surely it is a length information
                {
                    currentLen = 0;
                    juteBuffer = null;
                    // get the length information from the stream
                    juteBuffer = new byte[ReadLength(bData)];
                    // try getting other info from the stream
                    stream.BeginRead(juteBuffer, 0, juteBuffer.Length, ReceiveAsynch, juteBuffer);
                }
                else // not an incoming buffer then it is surely a zookeeper process information
                {
                    if (Interlocked.CompareExchange(ref initialized, 1, 0) == 0)
                    {
                        // haven't been initialized so read the authentication negotiation result
                        ReadConnectResult(bData);
                        // reading length information
                        stream.BeginRead(incomingBuffer, 0, incomingBuffer.Length, ReceiveAsynch, incomingBuffer);
                    }
                    else
                    {
                        currentLen += len;
                        if (juteBuffer.Length > currentLen) // stream haven't been completed so read any left bytes
                            stream.BeginRead(juteBuffer, currentLen, juteBuffer.Length - currentLen, ReceiveAsynch, juteBuffer);
                        else
                        {
                            // stream is complete so read the response
                            ReadResponse(bData);
                            // everything is fine, now read the stream again for length information
                            stream.BeginRead(incomingBuffer, 0, incomingBuffer.Length, ReceiveAsynch, incomingBuffer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrimeConnection()
        {
            log.InfoFormat("Socket connection established to {0}, initiating session", client.Client.RemoteEndPoint);
            ConnectRequest conReq = new ConnectRequest(0, lastZxid, Convert.ToInt32(conn.SessionTimeout.TotalMilliseconds), conn.SessionId, conn.SessionPassword);
            lock (outgoingQueue)
            {
                if (!ClientConnection.DisableAutoWatchReset && (!zooKeeper.DataWatches.IsEmpty() || !zooKeeper.ExistWatches.IsEmpty() || !zooKeeper.ChildWatches.IsEmpty()))
                {
                    var sw = new SetWatches(lastZxid, zooKeeper.DataWatches, zooKeeper.ExistWatches, zooKeeper.ChildWatches);
                    var h = new RequestHeader();
                    h.Type = (int)OpCode.SetWatches;
                    h.Xid = -8;
                    Packet packet = new Packet(h, new ReplyHeader(), sw, null, null, null, null, null);
                    //outgoingQueue.AddFirst(packet);
                    addPacketFirst(packet);
                }

                foreach (ClientConnection.AuthData id in conn.authInfo)
                    addPacketFirst(
                        new Packet(new RequestHeader(-4, (int)OpCode.Auth), null, new AuthPacket(0, id.Scheme, id.GetData()), null, null, null, null, null));

                addPacketFirst(new Packet(null, null, conReq, null, null, null, null, null));

            }
            packetAre.Set();
            if (log.IsDebugEnabled)
                log.DebugFormat("Session establishment request sent on {0}", client.Client.RemoteEndPoint);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendPing()
        {
            lastPingSentNs = DateTime.UtcNow.Nanos();
            RequestHeader h = new RequestHeader(-2, (int)OpCode.Ping);
            conn.QueuePacket(h, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        /// send packet to server        
        /// there's posibility when server closed the socket and client try to send some packet, when this happen it will throw exception
        /// the exception is either IOException, NullReferenceException and/or ObjectDisposedException
        /// so it is mandatory to catch these excepetions
        /// </summary>
        /// <param name="packet">The packet to send</param>
        private void DoSend(Packet packet)
        {
            if (packet.header != null
                && packet.header.Type != (int)OpCode.Ping
                && packet.header.Type != (int)OpCode.Auth)
            {
                pendingQueue.Enqueue(packet);
            }
            client.GetStream().Write(packet.data, 0, packet.data.Length);
            sentCount++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static int ReadLength(byte[] content)
        {
            using (EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Big, new MemoryStream(content), Encoding.UTF8))
            {
                int len = reader.ReadInt32();
                if (len < 0 || len >= ClientConnection.MaximumPacketLength)
                    throw new IOException(new StringBuilder("Packet len ").Append(len).Append("is out of range!").ToString());
                return len;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        private void ReadConnectResult(byte[] content)
        {
            using (var reader = new EndianBinaryReader(EndianBitConverter.Big, new MemoryStream(content), Encoding.UTF8))
            {
                zkEndpoints.CurrentEndPoint.SetAsSuccess();
                BinaryInputArchive bbia = BinaryInputArchive.GetArchive(reader);
                ConnectResponse conRsp = new ConnectResponse();
                conRsp.Deserialize(bbia, "connect");
                negotiatedSessionTimeout = conRsp.TimeOut;
                if (negotiatedSessionTimeout <= 0)
                {
                    zooKeeper.State = States.CLOSED;
                    conn.consumer.QueueEvent(new WatchedEvent(KeeperState.Expired, EventType.None, null));
                    throw new SessionExpiredException(new StringBuilder().AppendFormat("Unable to reconnect to ZooKeeper service, session 0x{0:X} has expired", conn.SessionId).ToString());
                }
                conn.readTimeout = new TimeSpan(0, 0, 0, 0, negotiatedSessionTimeout * 2 / 3);
                // commented...we haven't need this information yet...
                //conn.connectTimeout = new TimeSpan(0, 0, 0, negotiatedSessionTimeout / conn.serverAddrs.Count);
                conn.SessionId = conRsp.SessionId;
                conn.SessionPassword = conRsp.Passwd;
                zooKeeper.State = States.CONNECTED;
                log.InfoFormat("Session establishment complete on server {0:X}, negotiated timeout = {1}", conn.SessionId, negotiatedSessionTimeout);
                conn.consumer.QueueEvent(new WatchedEvent(KeeperState.SyncConnected, EventType.None, null));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        private void ReadResponse(byte[] content)
        {
            using (var reader = new EndianBinaryReader(EndianBitConverter.Big, new MemoryStream(content), Encoding.UTF8))
            {
                BinaryInputArchive bbia = BinaryInputArchive.GetArchive(reader);
                ReplyHeader replyHdr = new ReplyHeader();

                replyHdr.Deserialize(bbia, "header");
                if (replyHdr.Xid == -2)
                {
                    // -2 is the xid for pings
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Got ping response for sessionid: 0x{0:X} after {1}ms", conn.SessionId, (DateTime.UtcNow.Nanos() - lastPingSentNs) / 1000000);
                    return;
                }
                if (replyHdr.Xid == -4)
                {
                    // -2 is the xid for AuthPacket
                    // TODO: process AuthPacket here
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Got auth sessionid:0x{0:X}", conn.SessionId);
                    return;
                }
                if (replyHdr.Xid == -1)
                {
                    // -1 means notification
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Got notification sessionid:0x{0}", conn.SessionId);

                    WatcherEvent @event = new WatcherEvent();
                    @event.Deserialize(bbia, "response");

                    // convert from a server path to a client path
                    if (conn.ChrootPath != null)
                    {
                        string serverPath = @event.Path;
                        if (serverPath.CompareTo(conn.ChrootPath) == 0)
                            @event.Path = PathUtils.PathSeparator;
                        else
                            @event.Path = serverPath.Substring(conn.ChrootPath.Length);
                    }

                    WatchedEvent we = new WatchedEvent(@event);
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Got {0} for sessionid 0x{1:X}", we, conn.SessionId);

                    conn.consumer.QueueEvent(we);
                    return;
                }
                Packet packet;
                if (pendingQueue.TryDequeue(out packet))
                {
                    try
                    {
                        if (packet.header.Xid != replyHdr.Xid)
                        {
                            packet.replyHeader.Err = (int)KeeperException.Code.CONNECTIONLOSS;
                            throw new IOException(new StringBuilder("Xid out of order. Got ").Append(replyHdr.Xid).Append(" expected ").Append(packet.header.Xid).ToString());
                        }

                        packet.replyHeader.Xid = replyHdr.Xid;
                        packet.replyHeader.Err = replyHdr.Err;
                        packet.replyHeader.Zxid = replyHdr.Zxid;
                        if (replyHdr.Zxid > 0)
                            lastZxid = replyHdr.Zxid;

                        if (packet.response != null && replyHdr.Err == 0)
                            packet.response.Deserialize(bbia, "response");

                        if (log.IsDebugEnabled)
                            log.DebugFormat("Reading reply sessionid:0x{0:X}, packet:: {1}", conn.SessionId, packet);
                    }
                    finally
                    {
                        FinishPacket(packet);
                    }
                }
                else
                {
                    throw new IOException(new StringBuilder("Nothing in the queue, but got ").Append(replyHdr.Xid).ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        private void ConLossPacket(Packet p)
        {
            if (p.replyHeader == null) return;

            string state = zooKeeper.State.State;
            if (state == States.AUTH_FAILED.State)
                p.replyHeader.Err = (int)KeeperException.Code.AUTHFAILED;
            else if (state == States.CLOSED.State)
                p.replyHeader.Err = (int)KeeperException.Code.SESSIONEXPIRED;
            else
                p.replyHeader.Err = (int)KeeperException.Code.CONNECTIONLOSS;

            FinishPacket(p);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        private static void FinishPacket(Packet p)
        {
            if (p.watchRegistration != null)
                p.watchRegistration.Register(p.replyHeader.Err);

            p.Finished = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private int isDisposed = 0;

        /// <summary>
        /// 
        /// </summary>
        private void InternalDispose()
        {
            if (Interlocked.CompareExchange(ref isDisposed, 1, 0) == 0)
            {
                zooKeeper.State = States.CLOSED;
                try
                {
                    if (requestThread.IsAlive)
                    {
                        requestThread.Join();
                    }
                }
                catch (Exception ex)
                {
                    log.WarnFormat("Error disposing {0} : {1}", this.GetType().FullName, ex.Message);
                }

                incomingBuffer = juteBuffer = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            InternalDispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        ~ClientConnectionRequestProducer()
        {
            InternalDispose();
        }
    }
}