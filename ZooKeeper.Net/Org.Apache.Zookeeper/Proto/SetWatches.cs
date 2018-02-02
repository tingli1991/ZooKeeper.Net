using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Proto
{
    /// <summary>
    /// 
    /// </summary>
    public class SetWatches : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(SetWatches));

        /// <summary>
        /// 
        /// </summary>
        public SetWatches()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeZxid"></param>
        /// <param name="dataWatches"></param>
        /// <param name="existWatches"></param>
        /// <param name="childWatches"></param>
        public SetWatches(long relativeZxid, System.Collections.Generic.IEnumerable<string> dataWatches, System.Collections.Generic.IEnumerable<string> existWatches, System.Collections.Generic.IEnumerable<string> childWatches)
        {
            RelativeZxid = relativeZxid;
            DataWatches = dataWatches;
            ExistWatches = existWatches;
            ChildWatches = childWatches;
        }

        /// <summary>
        /// 
        /// </summary>
        public long RelativeZxid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> DataWatches { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> ExistWatches { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> ChildWatches { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, string tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteLong(RelativeZxid, "relativeZxid");
            {
                a_.StartVector(DataWatches, "dataWatches");
                if (DataWatches != null)
                {
                    foreach (var e1 in DataWatches)
                    {
                        a_.WriteString(e1, e1);
                    }
                }
                a_.EndVector(DataWatches, "dataWatches");
            }
            {
                a_.StartVector(ExistWatches, "existWatches");
                if (ExistWatches != null)
                {
                    foreach (var e1 in ExistWatches)
                    {
                        a_.WriteString(e1, e1);
                    }
                }
                a_.EndVector(ExistWatches, "existWatches");
            }
            {
                a_.StartVector(ChildWatches, "childWatches");
                if (ChildWatches != null)
                {
                    foreach (var e1 in ChildWatches)
                    {
                        a_.WriteString(e1, e1);
                    }
                }
                a_.EndVector(ChildWatches, "childWatches");
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
            RelativeZxid = a_.ReadLong("relativeZxid");
            {
                IIndex vidx1 = a_.StartVector("dataWatches");
                if (vidx1 != null)
                {
                    var tmpLst = new System.Collections.Generic.List<string>();
                    for (; !vidx1.Done(); vidx1.Incr())
                    {
                        string e1;
                        e1 = a_.ReadString("e1");
                        tmpLst.Add(e1);
                    }
                    DataWatches = tmpLst;
                }
                a_.EndVector("dataWatches");
            }
            {
                IIndex vidx1 = a_.StartVector("existWatches");
                if (vidx1 != null)
                {
                    var tmpLst = new System.Collections.Generic.List<string>();
                    for (; !vidx1.Done(); vidx1.Incr())
                    {
                        string e1;
                        e1 = a_.ReadString("e1");
                        tmpLst.Add(e1);
                    }
                    ExistWatches = tmpLst;
                }
                a_.EndVector("existWatches");
            }
            {
                IIndex vidx1 = a_.StartVector("childWatches");
                if (vidx1 != null)
                {
                    var tmpLst = new System.Collections.Generic.List<string>();
                    for (; !vidx1.Done(); vidx1.Incr())
                    {
                        string e1;
                        e1 = a_.ReadString("e1");
                        tmpLst.Add(e1);
                    }
                    ChildWatches = tmpLst;
                }
                a_.EndVector("childWatches");
            }
            a_.EndRecord(tag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (EndianBinaryWriter writer = new EndianBinaryWriter(EndianBitConverter.Big, ms, System.Text.Encoding.UTF8))
                {
                    BinaryOutputArchive a_ = new BinaryOutputArchive(writer);
                    a_.StartRecord(this, string.Empty);
                    a_.WriteLong(RelativeZxid, "relativeZxid");
                    {
                        a_.StartVector(DataWatches, "dataWatches");
                        if (DataWatches != null)
                        {
                            foreach (var e1 in DataWatches)
                            {
                                a_.WriteString(e1, e1);
                            }
                        }
                        a_.EndVector(DataWatches, "dataWatches");
                    }
                    {
                        a_.StartVector(ExistWatches, "existWatches");
                        if (ExistWatches != null)
                        {
                            foreach (var e1 in ExistWatches)
                            {
                                a_.WriteString(e1, e1);
                            }
                        }
                        a_.EndVector(ExistWatches, "existWatches");
                    }
                    {
                        a_.StartVector(ChildWatches, "childWatches");
                        if (ChildWatches != null)
                        {
                            foreach (var e1 in ChildWatches)
                            {
                                a_.WriteString(e1, e1);
                            }
                        }
                        a_.EndVector(ChildWatches, "childWatches");
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
            throw new InvalidOperationException("comparing SetWatches is unimplemented");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            SetWatches peer = (SetWatches)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = (RelativeZxid == peer.RelativeZxid);
            if (!ret) return ret;
            ret = DataWatches.Equals(peer.DataWatches);
            if (!ret) return ret;
            ret = ExistWatches.Equals(peer.ExistWatches);
            if (!ret) return ret;
            ret = ChildWatches.Equals(peer.ChildWatches);
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
            ret = (int)RelativeZxid;
            result = 37 * result + ret;
            ret = DataWatches.GetHashCode();
            result = 37 * result + ret;
            ret = ExistWatches.GetHashCode();
            result = 37 * result + ret;
            ret = ChildWatches.GetHashCode();
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LSetWatches(l[s][s][s])";
        }
    }
}