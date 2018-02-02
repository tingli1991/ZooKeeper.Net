using System;
using System.Threading;
using ZooKeeper.Net;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SleepingRetry : IRetryPolicy
    {
        private readonly int _n;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        protected SleepingRetry(int n)
        {
            _n = n;
        }

        /// <summary>
        /// 
        /// </summary>
        public int N { get { return _n; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retryCount"></param>
        /// <param name="elapsedTime"></param>
        /// <param name="retrySleeper"></param>
        /// <returns></returns>
        public bool AllowRetry(int retryCount, TimeSpan elapsedTime, Action<TimeSpan> retrySleeper = null)
        {
            var allow = false;
            var effectiveSleeper = retrySleeper ?? Thread.Sleep;

            if (retryCount < _n)
            {
                try
                {
                    effectiveSleeper(GetSleepTime(retryCount, elapsedTime));
                }
                catch (ThreadInterruptedException)
                {
                    // Handle gracefully and don't allow retries
                }
                allow = true;
            }
            return allow;
        }
        protected abstract TimeSpan GetSleepTime(int retryCount, TimeSpan elapsedTime);
    }

    /// <summary>
    /// 
    /// </summary>
    public class RetryNTimes : SleepingRetry
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly TimeSpan _sleepTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="sleepTime"></param>
        public RetryNTimes(int n, TimeSpan sleepTime)
            : base(n)
        {
            _sleepTime = sleepTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retryCount"></param>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        protected override TimeSpan GetSleepTime(int retryCount, TimeSpan elapsedTime)
        {
            return _sleepTime;
        }
    }
}