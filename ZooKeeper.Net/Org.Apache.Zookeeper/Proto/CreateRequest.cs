using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Proto
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateRequest : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(CreateRequest));

        /// <summary>
        /// 
        /// </summary>
        public CreateRequest()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="acl"></param>
        /// <param name="flags"></param>
        public CreateRequest(string path, byte[] data, System.Collections.Generic.IEnumerable<Data.ACL> acl, int flags)
        {
            Path = path;
            Data = data;
            Acl = acl;
            Flags = flags;
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
        public System.Collections.Generic.IEnumerable<Data.ACL> Acl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, String tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteString(Path, "path");
            a_.WriteBuffer(Data, "data");
            {
                a_.StartVector(Acl, "acl");
                if (Acl != null)
                {
                    foreach (var e1 in Acl)
                    {
                        a_.WriteRecord(e1, "e1");
                    }
                }
                a_.EndVector(Acl, "acl");
            }
            a_.WriteInt(Flags, "flags");
            a_.EndRecord(this, tag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Deserialize(IInputArchive a_, String tag)
        {
            a_.StartRecord(tag);
            Path = a_.ReadString("path");
            Data = a_.ReadBuffer("data");
            {
                IIndex vidx1 = a_.StartVector("acl");
                if (vidx1 != null)
                {
                    var tmpLst = new System.Collections.Generic.List<Data.ACL>();
                    for (; !vidx1.Done(); vidx1.Incr())
                    {
                        Data.ACL e1;
                        e1 = new Data.ACL();
                        a_.ReadRecord(e1, "e1");
                        tmpLst.Add(e1);
                    }
                    Acl = tmpLst;
                }
                a_.EndVector("acl");
            }
            Flags = a_.ReadInt("flags");
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
                    {
                        a_.StartVector(Acl, "acl");
                        if (Acl != null)
                        {
                            foreach (var e1 in Acl)
                            {
                                a_.WriteRecord(e1, "e1");
                            }
                        }
                        a_.EndVector(Acl, "acl");
                    }
                    a_.WriteInt(Flags, "flags");
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
            throw new InvalidOperationException("comparing CreateRequest is unimplemented");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            CreateRequest peer = (CreateRequest)obj;
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
            ret = Acl.Equals(peer.Acl);
            if (!ret) return ret;
            ret = (Flags == peer.Flags);
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
            ret = Acl.GetHashCode();
            result = 37 * result + ret;
            ret = (int)Flags;
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LCreateRequest(sB[LACL(iLId(ss))]i)";
        }
    }
}