using ZooKeeper.Net.IO;
using System.IO;
using System.Text;
using ZooKeeper.Net;

namespace Org.Apache.Jute
{
    /// <summary>
    /// 
    /// </summary>
    public class BinaryInputArchive : IInputArchive
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly EndianBinaryReader reader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        static public BinaryInputArchive GetArchive(EndianBinaryReader reader)
        {
            return new BinaryInputArchive(reader);
        }

        /// <summary>
        /// 
        /// </summary>
        private class BinaryIndex : IIndex
        {
            /// <summary>
            /// 
            /// </summary>
            private int nelems;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="nelems"></param>
            internal BinaryIndex(int nelems)
            {
                this.nelems = nelems;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool Done()
            {
                return (nelems <= 0);
            }

            /// <summary>
            /// 
            /// </summary>
            public void Incr()
            {
                nelems--;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public BinaryInputArchive(EndianBinaryReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public byte ReadByte(string tag)
        {
            return reader.ReadByte();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool ReadBool(string tag)
        {
            return reader.ReadBoolean();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public int ReadInt(string tag)
        {
            return reader.ReadInt32();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public long ReadLong(string tag)
        {
            return reader.ReadInt64();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public float ReadFloat(string tag)
        {
            return reader.ReadSingle();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public double ReadDouble(string tag)
        {
            return reader.ReadDouble();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public string ReadString(string tag)
        {
            int len = reader.ReadInt32();
            if (len == -1) return null;
            var b = reader.ReadBytesOrThrow(len);
            return Encoding.UTF8.GetString(b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public byte[] ReadBuffer(string tag)
        {
            int len = ReadInt(tag);
            if (len == -1) return null;
            if (len < 0 || len > ClientConnection.MaximumPacketLength)
            {
                throw new IOException(new StringBuilder("Unreasonable length = ").Append(len).ToString());
            }
            var arr = reader.ReadBytesOrThrow(len);
            return arr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="tag"></param>
        public void ReadRecord(IRecord r, string tag)
        {
            r.Deserialize(this, tag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>

        public void StartRecord(string tag) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public void EndRecord(string tag) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public IIndex StartVector(string tag)
        {
            int len = ReadInt(tag);
            if (len == -1)
            {
                return null;
            }
            return new BinaryIndex(len);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public void EndVector(string tag) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public IIndex StartMap(string tag)
        {
            return new BinaryIndex(ReadInt(tag));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public void EndMap(string tag) { }

    }
}