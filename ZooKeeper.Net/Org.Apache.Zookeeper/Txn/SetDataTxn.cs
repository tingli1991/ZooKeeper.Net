using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Txn
{
    /// <summary>
    /// 
    /// </summary>
    public class SetDataTxn : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(SetDataTxn));

        /// <summary>
        /// 
        /// </summary>
        public SetDataTxn()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="version"></param>
        public SetDataTxn(string path, byte[] data, int version)
        {
            Path = path;
            Data = data;
            Version = version;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, string tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteString(Path, "path");
            a_.WriteBuffer(Data, "data");
            a_.WriteInt(Version, "version");
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
            Path = a_.ReadString("path");
            Data = a_.ReadBuffer("data");
            Version = a_.ReadInt("version");
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
                    a_.WriteString(Path, "path");
                    a_.WriteBuffer(Data, "data");
                    a_.WriteInt(Version, "version");
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
            SetDataTxn peer = (SetDataTxn)obj;
            if (peer == null)
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            int ret = 0;
            ret = Path.CompareTo(peer.Path);
            if (ret != 0) return ret;
            ret = Data.CompareTo(peer.Data);
            if (ret != 0) return ret;
            ret = (Version == peer.Version) ? 0 : ((Version < peer.Version) ? -1 : 1);
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
            SetDataTxn peer = (SetDataTxn)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = Path.Equals(peer.Path);
            if (!ret) return ret;
            ret = Data.Equals(peer.Data);
            if (!ret) return ret;
            ret = (Version == peer.Version);
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
            ret = Path.GetHashCode();
            result = 37 * result + ret;
            ret = Data.GetHashCode();
            result = 37 * result + ret;
            ret = (int)Version;
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LSetDataTxn(sBi)";
        }
    }
}