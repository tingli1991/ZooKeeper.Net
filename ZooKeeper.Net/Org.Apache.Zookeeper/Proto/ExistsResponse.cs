using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Proto
{
    /// <summary>
    /// 
    /// </summary>
    public class ExistsResponse : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(ExistsResponse));

        /// <summary>
        /// 
        /// </summary>
        public ExistsResponse()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stat"></param>
        public ExistsResponse(Data.Stat stat)
        {
            Stat = stat;
        }

        /// <summary>
        /// 
        /// </summary>
        public Data.Stat Stat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, string tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteRecord(Stat, "stat");
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
            Stat = new Data.Stat();
            a_.ReadRecord(Stat, "stat");
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
                    a_.WriteRecord(Stat, "stat");
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
            ExistsResponse peer = (ExistsResponse)obj;
            if (peer == null)
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            int ret = 0;
            ret = Stat.CompareTo(peer.Stat);
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
            ExistsResponse peer = (ExistsResponse)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = Stat.Equals(peer.Stat);
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
            ret = Stat.GetHashCode();
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LExistsResponse(LStat(lllliiiliil))";
        }
    }
}