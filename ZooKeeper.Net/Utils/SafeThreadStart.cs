using System;
using log4net;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class SafeThreadStart
    {
        private readonly Action action;
        private static readonly ILog log = LogManager.GetLogger(typeof(SafeThreadStart));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public SafeThreadStart(Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Run()
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                log.Error("Unhandled exception in background thread", e);
            }
        }
    }
}