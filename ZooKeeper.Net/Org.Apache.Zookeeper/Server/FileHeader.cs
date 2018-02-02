using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Server.Persistence
{
    /// <summary>
    /// 
    /// </summary>
    public class FileHeader : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(FileHeader));

        /// <summary>
        /// 
        /// </summary>
        public FileHeader()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="magic"></param>
        /// <param name="version"></param>
        /// <param name="dbid"></param>
        public FileHeader(int magic, int version, long dbid)
        {
            Magic = magic;
            Version = version;
            Dbid = dbid;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Magic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Dbid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, String tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteInt(Magic, "magic");
            a_.WriteInt(Version, "version");
            a_.WriteLong(Dbid, "dbid");
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
            Magic = a_.ReadInt("magic");
            Version = a_.ReadInt("version");
            Dbid = a_.ReadLong("dbid");
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
                    a_.WriteInt(Magic, "magic");
                    a_.WriteInt(Version, "version");
                    a_.WriteLong(Dbid, "dbid");
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
            FileHeader peer = (FileHeader)obj;
            if (peer == null)
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            int ret = 0;
            ret = (Magic == peer.Magic) ? 0 : ((Magic < peer.Magic) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Version == peer.Version) ? 0 : ((Version < peer.Version) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Dbid == peer.Dbid) ? 0 : ((Dbid < peer.Dbid) ? -1 : 1);
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
            FileHeader peer = (FileHeader)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = (Magic == peer.Magic);
            if (!ret) return ret;
            ret = (Version == peer.Version);
            if (!ret) return ret;
            ret = (Dbid == peer.Dbid);
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
            ret = (int)Magic;
            result = 37 * result + ret;
            ret = (int)Version;
            result = 37 * result + ret;
            ret = (int)Dbid;
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LFileHeader(iil)";
        }
    }
}