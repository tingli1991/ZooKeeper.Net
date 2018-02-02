using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Server.Quorum
{
    /// <summary>
    /// 
    /// </summary>
    public class QuorumPacket : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(QuorumPacket));

        /// <summary>
        /// 
        /// </summary>
        public QuorumPacket()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="zxid"></param>
        /// <param name="data"></param>
        /// <param name="authinfo"></param>
        public QuorumPacket(int type, long zxid, byte[] data, System.Collections.Generic.IEnumerable<Data.ZKId> authinfo)
        {
            Type = type;
            Zxid = zxid;
            Data = data;
            Authinfo = authinfo;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Zxid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.Generic.IEnumerable<Data.ZKId> Authinfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, string tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteInt(Type, "type");
            a_.WriteLong(Zxid, "zxid");
            a_.WriteBuffer(Data, "data");
            {
                a_.StartVector(Authinfo, "authinfo");
                if (Authinfo != null)
                {
                    foreach (var e1 in Authinfo)
                    {
                        a_.WriteRecord(e1, "e1");
                    }
                }
                a_.EndVector(Authinfo, "authinfo");
            }
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
            Type = a_.ReadInt("type");
            Zxid = a_.ReadLong("zxid");
            Data = a_.ReadBuffer("data");
            {
                IIndex vidx1 = a_.StartVector("authinfo");
                if (vidx1 != null)
                {
                    var tmpLst = new System.Collections.Generic.List<Data.ZKId>();
                    for (; !vidx1.Done(); vidx1.Incr())
                    {
                        Data.ZKId e1;
                        e1 = new Data.ZKId();
                        a_.ReadRecord(e1, "e1");
                        tmpLst.Add(e1);
                    }
                    Authinfo = tmpLst;
                }
                a_.EndVector("authinfo");
            }
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
                using (EndianBinaryWriter writer =
                  new EndianBinaryWriter(EndianBitConverter.Big, ms, System.Text.Encoding.UTF8))
                {
                    BinaryOutputArchive a_ = new BinaryOutputArchive(writer);
                    a_.StartRecord(this, string.Empty);
                    a_.WriteInt(Type, "type");
                    a_.WriteLong(Zxid, "zxid");
                    a_.WriteBuffer(Data, "data");
                    {
                        a_.StartVector(Authinfo, "authinfo");
                        if (Authinfo != null)
                        {
                            foreach (var e1 in Authinfo)
                            {
                                a_.WriteRecord(e1, "e1");
                            }
                        }
                        a_.EndVector(Authinfo, "authinfo");
                    }
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
            throw new InvalidOperationException("comparing QuorumPacket is unimplemented");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            QuorumPacket peer = (QuorumPacket)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = (Type == peer.Type);
            if (!ret) return ret;
            ret = (Zxid == peer.Zxid);
            if (!ret) return ret;
            ret = Data.Equals(peer.Data);
            if (!ret) return ret;
            ret = Authinfo.Equals(peer.Authinfo);
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
            ret = (int)Type;
            result = 37 * result + ret;
            ret = (int)Zxid;
            result = 37 * result + ret;
            ret = Data.GetHashCode();
            result = 37 * result + ret;
            ret = Authinfo.GetHashCode();
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LQuorumPacket(ilB[LId(ss)])";
        }
    }
}