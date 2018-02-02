using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class Stat : IRecord, IComparable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Stat));

        /// <summary>
        /// 
        /// </summary>
        public Stat()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="czxid"></param>
        /// <param name="mzxid"></param>
        /// <param name="ctime"></param>
        /// <param name="mtime"></param>
        /// <param name="version"></param>
        /// <param name="cversion"></param>
        /// <param name="aversion"></param>
        /// <param name="ephemeralOwner"></param>
        /// <param name="dataLength"></param>
        /// <param name="numChildren"></param>
        /// <param name="pzxid"></param>
        public Stat(long czxid, long mzxid, long ctime, long mtime, int version, int cversion, int aversion, long ephemeralOwner, int dataLength, int numChildren, long pzxid)
        {
            Czxid = czxid;
            Mzxid = mzxid;
            Ctime = ctime;
            Mtime = mtime;
            Version = version;
            Cversion = cversion;
            Aversion = aversion;
            EphemeralOwner = ephemeralOwner;
            DataLength = dataLength;
            NumChildren = numChildren;
            Pzxid = pzxid;
        }

        /// <summary>
        /// 
        /// </summary>
        public long Czxid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Mzxid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Ctime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Mtime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Cversion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Aversion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long EphemeralOwner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DataLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NumChildren { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Pzxid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, String tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteLong(Czxid, "czxid");
            a_.WriteLong(Mzxid, "mzxid");
            a_.WriteLong(Ctime, "ctime");
            a_.WriteLong(Mtime, "mtime");
            a_.WriteInt(Version, "version");
            a_.WriteInt(Cversion, "cversion");
            a_.WriteInt(Aversion, "aversion");
            a_.WriteLong(EphemeralOwner, "ephemeralOwner");
            a_.WriteInt(DataLength, "dataLength");
            a_.WriteInt(NumChildren, "numChildren");
            a_.WriteLong(Pzxid, "pzxid");
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
            Czxid = a_.ReadLong("czxid");
            Mzxid = a_.ReadLong("mzxid");
            Ctime = a_.ReadLong("ctime");
            Mtime = a_.ReadLong("mtime");
            Version = a_.ReadInt("version");
            Cversion = a_.ReadInt("cversion");
            Aversion = a_.ReadInt("aversion");
            EphemeralOwner = a_.ReadLong("ephemeralOwner");
            DataLength = a_.ReadInt("dataLength");
            NumChildren = a_.ReadInt("numChildren");
            Pzxid = a_.ReadLong("pzxid");
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
                    a_.WriteLong(Czxid, "czxid");
                    a_.WriteLong(Mzxid, "mzxid");
                    a_.WriteLong(Ctime, "ctime");
                    a_.WriteLong(Mtime, "mtime");
                    a_.WriteInt(Version, "version");
                    a_.WriteInt(Cversion, "cversion");
                    a_.WriteInt(Aversion, "aversion");
                    a_.WriteLong(EphemeralOwner, "ephemeralOwner");
                    a_.WriteInt(DataLength, "dataLength");
                    a_.WriteInt(NumChildren, "numChildren");
                    a_.WriteLong(Pzxid, "pzxid");
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
            Stat peer = (Stat)obj;
            if (peer == null)
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            int ret = 0;
            ret = (Czxid == peer.Czxid) ? 0 : ((Czxid < peer.Czxid) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Mzxid == peer.Mzxid) ? 0 : ((Mzxid < peer.Mzxid) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Ctime == peer.Ctime) ? 0 : ((Ctime < peer.Ctime) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Mtime == peer.Mtime) ? 0 : ((Mtime < peer.Mtime) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Version == peer.Version) ? 0 : ((Version < peer.Version) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Cversion == peer.Cversion) ? 0 : ((Cversion < peer.Cversion) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Aversion == peer.Aversion) ? 0 : ((Aversion < peer.Aversion) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (EphemeralOwner == peer.EphemeralOwner) ? 0 : ((EphemeralOwner < peer.EphemeralOwner) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (DataLength == peer.DataLength) ? 0 : ((DataLength < peer.DataLength) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (NumChildren == peer.NumChildren) ? 0 : ((NumChildren < peer.NumChildren) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Pzxid == peer.Pzxid) ? 0 : ((Pzxid < peer.Pzxid) ? -1 : 1);
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
            Stat peer = (Stat)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = (Czxid == peer.Czxid);
            if (!ret) return ret;
            ret = (Mzxid == peer.Mzxid);
            if (!ret) return ret;
            ret = (Ctime == peer.Ctime);
            if (!ret) return ret;
            ret = (Mtime == peer.Mtime);
            if (!ret) return ret;
            ret = (Version == peer.Version);
            if (!ret) return ret;
            ret = (Cversion == peer.Cversion);
            if (!ret) return ret;
            ret = (Aversion == peer.Aversion);
            if (!ret) return ret;
            ret = (EphemeralOwner == peer.EphemeralOwner);
            if (!ret) return ret;
            ret = (DataLength == peer.DataLength);
            if (!ret) return ret;
            ret = (NumChildren == peer.NumChildren);
            if (!ret) return ret;
            ret = (Pzxid == peer.Pzxid);
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
            ret = (int)Czxid;
            result = 37 * result + ret;
            ret = (int)Mzxid;
            result = 37 * result + ret;
            ret = (int)Ctime;
            result = 37 * result + ret;
            ret = (int)Mtime;
            result = 37 * result + ret;
            ret = (int)Version;
            result = 37 * result + ret;
            ret = (int)Cversion;
            result = 37 * result + ret;
            ret = (int)Aversion;
            result = 37 * result + ret;
            ret = (int)EphemeralOwner;
            result = 37 * result + ret;
            ret = (int)DataLength;
            result = 37 * result + ret;
            ret = (int)NumChildren;
            result = 37 * result + ret;
            ret = (int)Pzxid;
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LStat(lllliiiliil)";
        }
    }
}