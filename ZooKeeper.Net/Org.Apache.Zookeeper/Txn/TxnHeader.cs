using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Txn
{
    /// <summary>
    /// 
    /// </summary>
    public class TxnHeader : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(TxnHeader));

        /// <summary>
        /// 
        /// </summary>
        public TxnHeader()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="cxid"></param>
        /// <param name="zxid"></param>
        /// <param name="time"></param>
        /// <param name="type"></param>
        public TxnHeader(long clientId, int cxid, long zxid, long time, int type)
        {
            ClientId = clientId;
            Cxid = cxid;
            Zxid = zxid;
            Time = time;
            Type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        public long ClientId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Cxid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Zxid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, string tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteLong(ClientId, "clientId");
            a_.WriteInt(Cxid, "cxid");
            a_.WriteLong(Zxid, "zxid");
            a_.WriteLong(Time, "time");
            a_.WriteInt(Type, "type");
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
            ClientId = a_.ReadLong("clientId");
            Cxid = a_.ReadInt("cxid");
            Zxid = a_.ReadLong("zxid");
            Time = a_.ReadLong("time");
            Type = a_.ReadInt("type");
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
                    a_.WriteLong(ClientId, "clientId");
                    a_.WriteInt(Cxid, "cxid");
                    a_.WriteLong(Zxid, "zxid");
                    a_.WriteLong(Time, "time");
                    a_.WriteInt(Type, "type");
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
            TxnHeader peer = (TxnHeader)obj;
            if (peer == null)
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            int ret = 0;
            ret = (ClientId == peer.ClientId) ? 0 : ((ClientId < peer.ClientId) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Cxid == peer.Cxid) ? 0 : ((Cxid < peer.Cxid) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Zxid == peer.Zxid) ? 0 : ((Zxid < peer.Zxid) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Time == peer.Time) ? 0 : ((Time < peer.Time) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Type == peer.Type) ? 0 : ((Type < peer.Type) ? -1 : 1);
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
            TxnHeader peer = (TxnHeader)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = (ClientId == peer.ClientId);
            if (!ret) return ret;
            ret = (Cxid == peer.Cxid);
            if (!ret) return ret;
            ret = (Zxid == peer.Zxid);
            if (!ret) return ret;
            ret = (Time == peer.Time);
            if (!ret) return ret;
            ret = (Type == peer.Type);
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
            ret = (int)ClientId;
            result = 37 * result + ret;
            ret = (int)Cxid;
            result = 37 * result + ret;
            ret = (int)Zxid;
            result = 37 * result + ret;
            ret = (int)Time;
            result = 37 * result + ret;
            ret = (int)Type;
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LTxnHeader(lilli)";
        }
    }
}