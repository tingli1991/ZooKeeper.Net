using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Proto
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectRequest : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(ConnectRequest));

        /// <summary>
        /// 
        /// </summary>
        public ConnectRequest()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="protocolVersion"></param>
        /// <param name="lastZxidSeen"></param>
        /// <param name="timeOut"></param>
        /// <param name="sessionId"></param>
        /// <param name="passwd"></param>
        public ConnectRequest(int protocolVersion, long lastZxidSeen, int timeOut, long sessionId, byte[] passwd)
        {
            ProtocolVersion = protocolVersion;
            LastZxidSeen = lastZxidSeen;
            TimeOut = timeOut;
            SessionId = sessionId;
            Passwd = passwd;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ProtocolVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long LastZxidSeen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long SessionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Passwd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, String tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteInt(ProtocolVersion, "protocolVersion");
            a_.WriteLong(LastZxidSeen, "lastZxidSeen");
            a_.WriteInt(TimeOut, "timeOut");
            a_.WriteLong(SessionId, "sessionId");
            a_.WriteBuffer(Passwd, "passwd");
            a_.EndRecord(this, tag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Deserialize(IInputArchive a_, String tag)
        {
            a_.StartRecord(tag);
            ProtocolVersion = a_.ReadInt("protocolVersion");
            LastZxidSeen = a_.ReadLong("lastZxidSeen");
            TimeOut = a_.ReadInt("timeOut");
            SessionId = a_.ReadLong("sessionId");
            Passwd = a_.ReadBuffer("passwd");
            a_.EndRecord(tag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (EndianBinaryWriter writer = new EndianBinaryWriter(EndianBitConverter.Big, ms, System.Text.Encoding.UTF8))
                {
                    BinaryOutputArchive a_ = new BinaryOutputArchive(writer);
                    a_.StartRecord(this, string.Empty);
                    a_.WriteInt(ProtocolVersion, "protocolVersion");
                    a_.WriteLong(LastZxidSeen, "lastZxidSeen");
                    a_.WriteInt(TimeOut, "timeOut");
                    a_.WriteLong(SessionId, "sessionId");
                    a_.WriteBuffer(Passwd, "passwd");
                    a_.EndRecord(this, string.Empty);
                    ms.Position = 0;
                    return System.Text.Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return "ERROR";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void Write(EndianBinaryWriter writer)
        {
            BinaryOutputArchive archive = new BinaryOutputArchive(writer);
            Serialize(archive, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadFields(EndianBinaryReader reader)
        {
            BinaryInputArchive archive = new BinaryInputArchive(reader);
            Deserialize(archive, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            ConnectRequest peer = (ConnectRequest)obj;
            if (peer == null)
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            int ret = 0;
            ret = (ProtocolVersion == peer.ProtocolVersion) ? 0 : ((ProtocolVersion < peer.ProtocolVersion) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (LastZxidSeen == peer.LastZxidSeen) ? 0 : ((LastZxidSeen < peer.LastZxidSeen) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (TimeOut == peer.TimeOut) ? 0 : ((TimeOut < peer.TimeOut) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (SessionId == peer.SessionId) ? 0 : ((SessionId < peer.SessionId) ? -1 : 1);
            if (ret != 0) return ret;
            ret = Passwd.CompareTo(peer.Passwd);
            if (ret != 0) return ret;
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            ConnectRequest peer = (ConnectRequest)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = (ProtocolVersion == peer.ProtocolVersion);
            if (!ret) return ret;
            ret = (LastZxidSeen == peer.LastZxidSeen);
            if (!ret) return ret;
            ret = (TimeOut == peer.TimeOut);
            if (!ret) return ret;
            ret = (SessionId == peer.SessionId);
            if (!ret) return ret;
            ret = Passwd.Equals(peer.Passwd);
            if (!ret) return ret;
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int result = 17;
            int ret = GetType().GetHashCode();
            result = 37 * result + ret;
            ret = ProtocolVersion;
            result = 37 * result + ret;
            ret = (int)LastZxidSeen;
            result = 37 * result + ret;
            ret = TimeOut;
            result = 37 * result + ret;
            ret = (int)SessionId;
            result = 37 * result + ret;
            ret = Passwd.GetHashCode();
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LConnectRequest(ililB)";
        }
    }
}