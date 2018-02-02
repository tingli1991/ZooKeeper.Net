namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public enum EventType
    {
        None = -1,
        NodeCreated = 1,
        NodeDeleted = 2,
        NodeDataChanged = 3,
        NodeChildrenChanged = 4
    }
}