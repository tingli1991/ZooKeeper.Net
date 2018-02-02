using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class ZKWatchManager : IClientWatchManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ZKWatchManager));
        internal readonly ConcurrentDictionary<string, HashSet<IWatcher>> dataWatches = new ConcurrentDictionary<string, HashSet<IWatcher>>();
        internal readonly ConcurrentDictionary<string, HashSet<IWatcher>> existWatches = new ConcurrentDictionary<string, HashSet<IWatcher>>();
        internal readonly ConcurrentDictionary<string, HashSet<IWatcher>> childWatches = new ConcurrentDictionary<string, HashSet<IWatcher>>();

        /// <summary>
        /// 
        /// </summary>
        private IWatcher defaultWatcher;

        /// <summary>
        /// 
        /// </summary>
        internal IWatcher DefaultWatcher
        {
            get
            {
                return Interlocked.CompareExchange(ref defaultWatcher, null, null);
            }
            set
            {
                Interlocked.Exchange(ref defaultWatcher, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private static void AddTo(HashSet<IWatcher> from, HashSet<IWatcher> to)
        {
            if (from == null) return;
            to.UnionWith(from);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="type"></param>
        /// <param name="clientPath"></param>
        /// <returns></returns>
        public IEnumerable<IWatcher> Materialize(KeeperState state, EventType type, string clientPath)
        {
            HashSet<IWatcher> result = new HashSet<IWatcher>();
            switch (type)
            {
                case EventType.None:
                    result.Add(defaultWatcher);
                    foreach (var ws in dataWatches.Values)
                    {
                        result.UnionWith(ws);
                    }
                    foreach (var ws in existWatches.Values)
                    {
                        result.UnionWith(ws);
                    }
                    foreach (var ws in childWatches.Values)
                    {
                        result.UnionWith(ws);
                    }

                    // clear the watches if auto watch reset is not enabled
                    if (ClientConnection.DisableAutoWatchReset &&
                        state != KeeperState.SyncConnected)
                    {
                        dataWatches.Clear();
                        existWatches.Clear();
                        childWatches.Clear();
                    }
                    return result;
                case EventType.NodeDataChanged:
                case EventType.NodeCreated:
                    AddTo(dataWatches.GetAndRemove(clientPath), result);
                    AddTo(existWatches.GetAndRemove(clientPath), result);
                    break;
                case EventType.NodeChildrenChanged:
                    AddTo(childWatches.GetAndRemove(clientPath), result);
                    break;
                case EventType.NodeDeleted:
                    AddTo(dataWatches.GetAndRemove(clientPath), result);
                    //XXX This shouldn't be needed, but just in case
                    HashSet<IWatcher> list = existWatches.GetAndRemove(clientPath);
                    if (list != null)
                    {
                        AddTo(existWatches.GetAndRemove(clientPath), result);
                        log.Warn("We are triggering an exists watch for delete! Shouldn't happen!");
                    }
                    AddTo(childWatches.GetAndRemove(clientPath), result);
                    break;
                default:
                    var msg = new StringBuilder("Unhandled watch event type ").Append(type).Append(" with state ").Append(state).Append(" on path ").Append(clientPath).ToString();
                    log.Error(msg);
                    throw new InvalidOperationException(msg);
            }
            return result;
        }
    }
}