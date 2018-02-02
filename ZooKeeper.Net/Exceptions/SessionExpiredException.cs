using System;
using System.IO;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    internal class SessionExpiredException : IOException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public SessionExpiredException(string msg) : base(msg)
        {
        }
    }
}