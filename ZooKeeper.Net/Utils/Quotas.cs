using System.Text;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Quotas
    {
        private Quotas() { }
        /** the zookeeper nodes that acts as the management and status node **/
        public const string procZookeeper = "/zookeeper";

        /** the zookeeper quota node that acts as the quota
         * management node for zookeeper */
        public const string quotaZookeeper = "/zookeeper/quota";

        /**
         * the limit node that has the limit of
         * a subtree
         */
        public const string limitNode = "zookeeper_limits";

        /**
         * the stat node that monitors the limit of
         * a subtree.
         */
        public const string statNode = "zookeeper_stats";

        /**
         * return the quota path associated with this
         * prefix
         * @param path the actual path in zookeeper.
         * @return the limit quota path
         */
        public static string QuotaPath(string path)
        {
            StringBuilder builder = new StringBuilder(quotaZookeeper)
            .Append(path)
            .Append(PathUtils.PathSeparator)
            .Append(limitNode);
            return builder.ToString();
        }

        /**
         * return the stat quota path associated with this
         * prefix.
         * @param path the actual path in zookeeper
         * @return the stat quota path
         */
        public static string StatPath(string path)
        {
            StringBuilder builder = new StringBuilder(quotaZookeeper)
            .Append(path)
            .Append(PathUtils.PathSeparator)
            .Append(statNode);
            return builder.ToString();
        }
    }
}