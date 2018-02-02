using System.Collections.Generic;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    internal class ChildWatchRegistration : WatchRegistration
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
        public ChildWatchRegistration(ZKWatchManager watchManager, IWatcher watcher, string clientPath)
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
            return watchManager.childWatches;
        }
    }
}