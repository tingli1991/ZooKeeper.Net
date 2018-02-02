using System;
using System.Linq;
using Org.Apache.Jute;
using log4net;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Server.Quorum
{
    /// <summary>
    /// 
    /// </summary>
    public class LearnerInfo : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(LearnerInfo));

        /// <summary>
        /// 
        /// </summary>
        public LearnerInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverid"></param>
        /// <param name="protocolVersion"></param>
        public LearnerInfo(long serverid, int protocolVersion)
        {
            Serverid = serverid;
            ProtocolVersion = protocolVersion;
        }

        /// <summary>
        /// 
        /// </summary>
        public long Serverid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ProtocolVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, String tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteLong(Serverid, "serverid");
            a_.WriteInt(ProtocolVersion, "protocolVersion");
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
            Serverid = a_.ReadLong("serverid");
            ProtocolVersion = a_.ReadInt("protocolVersion");
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
                    BinaryOutputArchive a_ =
                      new BinaryOutputArchive(writer);
                    a_.StartRecord(this, string.Empty);
                    a_.WriteLong(Serverid, "serverid");
                    a_.WriteInt(ProtocolVersion, "protocolVersion");
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
            LearnerInfo peer = (LearnerInfo)obj;
            if (peer == null)
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            int ret = 0;
            ret = (Serverid == peer.Serverid) ? 0 : ((Serverid < peer.Serverid) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (ProtocolVersion == peer.ProtocolVersion) ? 0 : ((ProtocolVersion < peer.ProtocolVersion) ? -1 : 1);
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
            LearnerInfo peer = (LearnerInfo)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = (Serverid == peer.Serverid);
            if (!ret) return ret;
            ret = (ProtocolVersion == peer.ProtocolVersion);
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
            ret = (int)Serverid;
            result = 37 * result + ret;
            ret = (int)ProtocolVersion;
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LLearnerInfo(li)";
        }
    }
}