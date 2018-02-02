namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public enum KeeperState
    {
        Unknown = -1,
        Disconnected = 0,
        NoSyncConnected = 1,
        SyncConnected = 3,
        Expired = -112
    }
}