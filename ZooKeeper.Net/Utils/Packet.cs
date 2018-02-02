using ZooKeeper.Net.IO;
using System.IO;
using System.Text;
using System.Threading;
using log4net;
using Org.Apache.Jute;
using Org.Apache.Zookeeper.Proto;
using System;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class Packet
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Packet));

        internal RequestHeader header;
        private string serverPath;
        internal ReplyHeader replyHeader;
        internal IRecord response;

        /// <summary>
        /// 
        /// </summary>
        internal WatchRegistration watchRegistration;
        internal readonly byte[] data;
        public readonly DateTime DateCreated;

        /** Client's view of the path (may differ due to chroot) **/
        private string clientPath;
        /** Servers's view of the path (may differ due to chroot) **/
        readonly IRecord request;

        internal Packet(RequestHeader header, ReplyHeader replyHeader, IRecord request, IRecord response, byte[] data, WatchRegistration watchRegistration, string serverPath, string clientPath)
        {
            this.header = header;
            this.replyHeader = replyHeader;
            this.request = request;
            this.response = response;
            this.serverPath = serverPath;
            this.clientPath = clientPath;
            if (data != null)
            {
                this.data = data;
            }
            else
            {
                try
                {
                    MemoryStream ms = new MemoryStream();
                    using (EndianBinaryWriter writer = new EndianBinaryWriter(EndianBitConverter.Big, ms, Encoding.UTF8))
                    {
                        BinaryOutputArchive boa = BinaryOutputArchive.getArchive(writer);
                        boa.WriteInt(-1, "len"); // We'll fill this in later
                        if (header != null)
                        {
                            header.Serialize(boa, "header");
                        }
                        if (request != null)
                        {
                            request.Serialize(boa, "request");
                        }
                        ms.Position = 0;
                        int len = Convert.ToInt32(ms.Length); // now we have the real length
                        writer.Write(len - 4); // update the length info
                        this.data = ms.ToArray();
                    }
                }
                catch (IOException e)
                {
                    log.Warn("Ignoring unexpected exception", e);
                }
            }
            this.watchRegistration = watchRegistration;
        }


        private readonly ManualResetEventSlim mreslim = new ManualResetEventSlim(false);
        public bool WaitUntilFinishedSlim(TimeSpan timeout)
        {
            return mreslim.Wait(timeout);
        }

        internal bool Finished
        {
            set
            {
                mreslim.Set();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("  clientPath:").Append(clientPath);
            sb.Append("  serverPath:").Append(serverPath);
            sb.Append("    finished:").Append(0);
            sb.Append("     header::").Append(header);
            sb.Append("replyHeader::").Append(replyHeader);
            sb.Append("    request::").Append(request);
            sb.Append("   response::").Append(response);

            // jute toString is horrible, remove unnecessary newlines
            return sb.ToString().Replace(@"\r*\n+", " ");
        }
    }
}