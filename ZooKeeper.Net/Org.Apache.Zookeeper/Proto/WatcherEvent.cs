using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Proto
{
    /// <summary>
    /// 
    /// </summary>
    public class WatcherEvent : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(WatcherEvent));

        /// <summary>
        /// 
        /// </summary>
        public WatcherEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="state"></param>
        /// <param name="path"></param>
        public WatcherEvent(int type, int state, string path)
        {
            Type = type;
            State = state;
            Path = path;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, string tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteInt(Type, "type");
            a_.WriteInt(State, "state");
            a_.WriteString(Path, "path");
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
            State = a_.ReadInt("state");
            Path = a_.ReadString("path");
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
                    a_.WriteInt(Type, "type");
                    a_.WriteInt(State, "state");
                    a_.WriteString(Path, "path");
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
            WatcherEvent peer = (WatcherEvent)obj;
            if (peer == null)
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            int ret = 0;
            ret = (Type == peer.Type) ? 0 : ((Type < peer.Type) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (State == peer.State) ? 0 : ((State < peer.State) ? -1 : 1);
            if (ret != 0) return ret;
            ret = Path.CompareTo(peer.Path);
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
            WatcherEvent peer = (WatcherEvent)obj;
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
            ret = (State == peer.State);
            if (!ret) return ret;
            ret = Path.Equals(peer.Path);
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
            ret = (int)State;
            result = 37 * result + ret;
            ret = Path.GetHashCode();
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LWatcherEvent(iis)";
        }
    }
}