using System.Collections.Generic;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    internal class DataWatchRegistration : WatchRegistration
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ZKWatchManager watchManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="watchManager"></param>
        /// <param name="watcher"></param>
        /// <param name="clientPath"></param>
        public DataWatchRegistration(ZKWatchManager watchManager, IWatcher watcher, string clientPath)
            : base(watcher, clientPath)
        {
            this.watchManager = watchManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        protected override IDictionary<string, HashSet<IWatcher>> GetWatches(int rc)
        {
            return watchManager.dataWatches;
        }
    }
}