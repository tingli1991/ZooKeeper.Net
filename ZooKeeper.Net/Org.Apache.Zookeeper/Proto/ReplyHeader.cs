using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Proto
{
    /// <summary>
    /// 
    /// </summary>
    public class ReplyHeader : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(ReplyHeader));

        /// <summary>
        /// 
        /// </summary>
        public ReplyHeader()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xid"></param>
        /// <param name="zxid"></param>
        /// <param name="err"></param>
        public ReplyHeader(int xid, long zxid, int err)
        {
            Xid = xid;
            Zxid = zxid;
            Err = err;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Xid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Zxid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Err { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, string tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteInt(Xid, "xid");
            a_.WriteLong(Zxid, "zxid");
            a_.WriteInt(Err, "err");
            a_.EndRecord(this, tag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Deserialize(IInputArchive a_, string tag)
        {
            a_.StartRecord(tag);
            Xid = a_.ReadInt("xid");
            Zxid = a_.ReadLong("zxid");
            Err = a_.ReadInt("err");
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
                    BinaryOutputArchive a_ =
                      new BinaryOutputArchive(writer);
                    a_.StartRecord(this, string.Empty);
                    a_.WriteInt(Xid, "xid");
                    a_.WriteLong(Zxid, "zxid");
                    a_.WriteInt(Err, "err");
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
        public void Write(EndianBinaryWriter writer)
        {
            BinaryOutputArchive archive = new BinaryOutputArchive(writer);
            Serialize(archive, string.Empty);
        }
        public void ReadFields(EndianBinaryReader reader)
        {
            BinaryInputArchive archive = new BinaryInputArchive(reader);
            Deserialize(archive, string.Empty);
        }
        public int CompareTo(object obj)
        {
            ReplyHeader peer = (ReplyHeader)obj;
            if (peer == null)
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            int ret = 0;
            ret = (Xid == peer.Xid) ? 0 : ((Xid < peer.Xid) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Zxid == peer.Zxid) ? 0 : ((Zxid < peer.Zxid) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Err == peer.Err) ? 0 : ((Err < peer.Err) ? -1 : 1);
            if (ret != 0) return ret;
            return ret;
        }
        public override bool Equals(object obj)
        {
            ReplyHeader peer = (ReplyHeader)obj;
            if (peer == null)
            {
                return false;
            }
            if (Object.ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = (Xid == peer.Xid);
            if (!ret) return ret;
            ret = (Zxid == peer.Zxid);
            if (!ret) return ret;
            ret = (Err == peer.Err);
            if (!ret) return ret;
            return ret;
        }
        public override int GetHashCode()
        {
            int result = 17;
            int ret = GetType().GetHashCode();
            result = 37 * result + ret;
            ret = (int)Xid;
            result = 37 * result + ret;
            ret = (int)Zxid;
            result = 37 * result + ret;
            ret = (int)Err;
            result = 37 * result + ret;
            return result;
        }
        public static string Signature()
        {
            return "LReplyHeader(ili)";
        }
    }
}
