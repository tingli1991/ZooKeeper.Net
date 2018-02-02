using ZooKeeper.Net.IO;

namespace Org.Apache.Jute
{
    /// <summary>
    /// 
    /// </summary>
    public class RecordWriter
    {
        private readonly BinaryOutputArchive archive;

        public RecordWriter(EndianBinaryWriter writer, string format)
        {
            archive = new BinaryOutputArchive(writer);
        }

        public void Write(IRecord r)
        {
            r.Serialize(archive, "");
        }
    }
}
