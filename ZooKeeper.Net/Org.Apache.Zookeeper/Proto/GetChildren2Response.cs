using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Proto
{
    /// <summary>
    /// 
    /// </summary>
    public class GetChildren2Response : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(GetChildren2Response));

        /// <summary>
        /// 
        /// </summary>
        public GetChildren2Response()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="children"></param>
        /// <param name="stat"></param>
        public GetChildren2Response(System.Collections.Generic.IEnumerable<string> children, Data.Stat stat)
        {
            Children = children;
            Stat = stat;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> Children { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Data.Stat Stat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, String tag)
        {
            a_.StartRecord(this, tag);
            {
                a_.StartVector(Children, "children");
                if (Children != null)
                {
                    foreach (var e1 in Children)
                    {
                        a_.WriteString(e1, e1);
                    }
                }
                a_.EndVector(Children, "children");
            }
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
            {
                IIndex vidx1 = a_.StartVector("children");
                if (vidx1 != null)
                {
                    var tmpLst = new System.Collections.Generic.List<string>();
                    for (; !vidx1.Done(); vidx1.Incr())
                    {
                        String e1;
                        e1 = a_.ReadString("e1");
                        tmpLst.Add(e1);
                    }
                    Children = tmpLst;
                }
                a_.EndVector("children");
            }
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
                using (EndianBinaryWriter writer = new EndianBinaryWriter(EndianBitConverter.Big, ms, System.Text.Encoding.UTF8))
                {
                    BinaryOutputArchive a_ = new BinaryOutputArchive(writer);
                    a_.StartRecord(this, string.Empty);
                    {
                        a_.StartVector(Children, "children");
                        if (Children != null)
                        {
                            foreach (var e1 in Children)
                            {
                                a_.WriteString(e1, e1);
                            }
                        }
                        a_.EndVector(Children, "children");
                    }
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
            throw new InvalidOperationException("comparing GetChildren2Response is unimplemented");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            GetChildren2Response peer = (GetChildren2Response)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = Children.Equals(peer.Children);
            if (!ret) return ret;
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
            ret = Children.GetHashCode();
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
            return "LGetChildren2Response([s]LStat(lllliiiliil))";
        }
    }
}