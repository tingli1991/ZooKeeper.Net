using System.Collections.Generic;
using log4net;
using System;
using System.Text;
using System.Threading;
using System.Linq;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public static class ZooKeeperEx
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(ZooKeeperEx));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetAndRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
                dictionary.Remove(key);

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long Nanos(this DateTime dateTime)
        {
            return dateTime.Ticks / 100;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return collection.Count() == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string @string)
        {
            return Encoding.UTF8.GetBytes(@string);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lock"></param>
        /// <returns></returns>
        public static IDisposable AcquireReadLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterReadLock();
            return new Disposable(@lock.ExitReadLock);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lock"></param>
        /// <returns></returns>
        public static IDisposable AcquireUpgradableReadLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterUpgradeableReadLock();
            return new Disposable(@lock.ExitUpgradeableReadLock);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lock"></param>
        /// <returns></returns>
        public static IDisposable AcquireWriteLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterWriteLock();
            return new Disposable(@lock.ExitWriteLock);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static string Combine(this string parent, string child)
        {
            StringBuilder builder = new StringBuilder(parent)
            .Append(PathUtils.PathSeparator)
            .Append(child);
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        private struct Disposable : IDisposable
        {
            private readonly Action action;
            private readonly Sentinel sentinel;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="action"></param>
            public Disposable(Action action)
            {
                this.action = action;
                sentinel = new Sentinel();
            }

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                try
                {
                    action();
                    sentinel.Dispose();
                }
                catch (Exception ex)
                {
                    log.WarnFormat("Error disposing {0} : {1}", this.GetType().FullName, ex.Message);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class Sentinel : IDisposable
        {
            ~Sentinel()
            {
                Dispose(false);
            }

            #region IDisposable Members
            /// <summary>
            /// 
            /// </summary>
            /// <param name="isDisposing"></param>
            private static void Dispose(bool isDisposing)
            {
                if (!isDisposing)
                    throw new InvalidOperationException("Lock not properly disposed.");
            }

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion
        }
    }
}