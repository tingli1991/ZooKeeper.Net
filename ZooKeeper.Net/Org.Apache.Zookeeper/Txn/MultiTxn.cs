using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Txn
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiTxn : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(MultiTxn));

        /// <summary>
        /// 
        /// </summary>
        public MultiTxn()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txns"></param>
        public MultiTxn(System.Collections.Generic.IEnumerable<Txn> txns)
        {
            Txns = txns;
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.Generic.IEnumerable<Txn> Txns { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, String tag)
        {
            a_.StartRecord(this, tag);
            {
                a_.StartVector(Txns, "txns");
                if (Txns != null)
                {
                    foreach (var e1 in Txns)
                    {
                        a_.WriteRecord(e1, "e1");
                    }
                }
                a_.EndVector(Txns, "txns");
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
            {
                IIndex vidx1 = a_.StartVector("txns");
                if (vidx1 != null)
                {
                    var tmpLst = new System.Collections.Generic.List<Txn>();
                    for (; !vidx1.Done(); vidx1.Incr())
                    {
                        Txn e1;
                        e1 = new Txn();
                        a_.ReadRecord(e1, "e1");
                        tmpLst.Add(e1);
                    }
                    Txns = tmpLst;
                }
                a_.EndVector("txns");
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
                    {
                        a_.StartVector(Txns, "txns");
                        if (Txns != null)
                        {
                            foreach (var e1 in Txns)
                            {
                                a_.WriteRecord(e1, "e1");
                            }
                        }
                        a_.EndVector(Txns, "txns");
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
            throw new InvalidOperationException("comparing MultiTxn is unimplemented");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            MultiTxn peer = (MultiTxn)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = Txns.Equals(peer.Txns);
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
            ret = Txns.GetHashCode();
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LMultiTxn([LTxn(iB)])";
        }
    }
}