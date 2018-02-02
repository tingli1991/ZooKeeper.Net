using ZooKeeper.Net.IO;

namespace Org.Apache.Jute
{
    public class RecordReader
    {
        private readonly IInputArchive archive;

        public RecordReader(EndianBinaryReader reader, string format)
        {
            archive = new BinaryInputArchive(reader);
        }

        public void Read(IRecord r)
        {
            r.Deserialize(archive, "");
        }

    }
}