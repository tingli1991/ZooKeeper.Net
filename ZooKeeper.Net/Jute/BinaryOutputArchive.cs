using ZooKeeper.Net.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Org.Apache.Jute
{
    /// <summary>
    /// 
    /// </summary>
    public class BinaryOutputArchive : IOutputArchive
    {
        private readonly EndianBinaryWriter writer;

        public static BinaryOutputArchive getArchive(EndianBinaryWriter writer)
        {
            return new BinaryOutputArchive(writer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public BinaryOutputArchive(EndianBinaryWriter writer)
        {
            this.writer = writer;
        }

        public void WriteByte(byte b, string tag)
        {
            writer.Write(b);
        }

        public void WriteBool(bool b, string tag)
        {
            writer.Write(b);
        }

        public void WriteInt(int i, string tag)
        {
            writer.Write(i);
        }

        public void WriteLong(long l, string tag)
        {
            writer.Write(l);
        }

        public void WriteFloat(float f, string tag)
        {
            writer.Write(f);
        }

        public void WriteDouble(double d, string tag)
        {
            writer.Write(d);
        }

        public void WriteString(string s, string tag)
        {
            if (s == null)
            {
                WriteInt(-1, "len");
                return;
            }
            byte[] bb = Encoding.UTF8.GetBytes(s);
            WriteInt(bb.Length, "len");
            writer.Write(bb, 0, bb.Length);
        }

        public void WriteBuffer(byte[] barr, string tag)
        {
            if (barr == null)
            {
                writer.Write(-1);
                return;
            }
            writer.Write(barr.Length);
            writer.Write(barr);
        }

        public void WriteRecord(IRecord r, string tag)
        {
            if (r == null) return;

            r.Serialize(this, tag);
        }

        public void StartRecord(IRecord r, string tag) { }

        public void EndRecord(IRecord r, string tag) { }

        public void StartVector<T>(IEnumerable<T> v, string tag)
        {
            if (v == null)
            {
                WriteInt(-1, tag);
                return;
            }
            WriteInt(v.Count(), tag);
        }

        public void EndVector<T>(IEnumerable<T> v, string tag) { }

        public void StartMap(SortedDictionary<string, string> v, string tag)
        {
            WriteInt(v.Count, tag);
        }

        public void EndMap(SortedDictionary<string, string> v, string tag) { }
    }
}