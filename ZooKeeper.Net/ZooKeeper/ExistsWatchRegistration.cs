using System.Collections.Generic;

namespace ZooKeeper.Net
{
    /// <summary>
    /// Handle the special case of exists watches - they add a watcher
    /// even in the case where NONODE result code is returned.
    /// </summary>
    internal class ExistsWatchRegistration : WatchRegistration
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
        public ExistsWatchRegistration(ZKWatchManager watchManager, IWatcher watcher, string clientPath)
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
            return rc == 0 ? watchManager.dataWatches : watchManager.existWatches;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        protected override bool ShouldAddWatch(int rc)
        {
            return rc == 0 || rc == (int)KeeperException.Code.NONODE;
        }
    }
}