using System.IO;
using System;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    internal class SessionTimeoutException : IOException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public SessionTimeoutException(string msg) : base(msg)
        {
        }
    }
}