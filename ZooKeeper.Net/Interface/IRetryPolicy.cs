using System;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRetryPolicy
    {
        bool AllowRetry(int retryCount, TimeSpan elapsedTime, Action<TimeSpan> retrySleeper = null);
    }
}