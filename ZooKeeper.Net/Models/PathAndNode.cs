namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class PathAndNode
    {
        public readonly string Path;
        public readonly string Node;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="node"></param>
        public PathAndNode(string path, string node)
        {
            Path = path;
            Node = node;
        }
    }
}