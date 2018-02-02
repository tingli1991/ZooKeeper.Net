namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        void Process(WatchedEvent @event);
    }
}