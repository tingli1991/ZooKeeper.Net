using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ZooKeeper.Net.Tests
{
    /// <summary>
    /// 自定义监听
    /// </summary>
    public class ConnectWatcher : IWatcher
    {
        volatile bool connected;//是否连接成功
        private static readonly object sync = new object();//对象锁
        private readonly ManualResetEvent resetEvent = new ManualResetEvent(false);//线程监听工具

        /// <summary>
        /// 
        /// </summary>
        public ConnectWatcher()
        {
            //复位ZooKeeper的连接状态
            Reset();
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        bool IsConnected()
        {
            return connected;
        }

        /// <summary>
        /// 复位ZooKeeper的连接状态
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Reset()
        {
            //将事件状态设置为有信号，从而允许一个或多个等待线程继续执行
            resetEvent.Set();

            //标记链接状态为断开状态
            connected = false;
        }

        /// <summary>
        /// 连接Zookeeper
        /// </summary>
        /// <param name="timeOut">超时时间</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Connected(TimeSpan timeOut)
        {
            var left = timeOut;//超时时间
            var expire = DateTime.UtcNow + timeOut;//过期时间
            while (!connected && left.TotalMilliseconds > 0)
            {
                lock (sync)
                {
                    Monitor.TryEnter(sync, left);
                }

                //递减时间
                left = expire - DateTime.UtcNow;
            }

            //验证是否超时
            if (!connected)
            {
                //抛出连接超时异常
                throw new TimeoutException("connection timed out");
            }
        }

        /// <summary>
        /// 断开链接
        /// </summary>
        /// <param name="timeOut"></param>
        private void Disconnected(TimeSpan timeOut)
        {
            var left = timeOut;//超时时间
            var expire = DateTime.UtcNow + timeOut;//过期时间
            while (connected && left.TotalMilliseconds > 0)
            {
                lock (sync)
                {
                    Monitor.TryEnter(sync, left);
                }

                //递减时间
                left = expire - DateTime.UtcNow;
            }

            //验证是否断开成功，如果没有断开成功返回断开超时
            if (connected)
            {
                //抛出连接超时异常
                throw new TimeoutException("connection timed out");
            }
        }

        /// <summary>
        /// 监听通知回调方法
        /// </summary>
        /// <param name="event"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Process(WatchedEvent @event)
        {
            if (@event.State == KeeperState.SyncConnected)
            {
                connected = true;
                lock (sync)
                {
                    Monitor.PulseAll(sync);
                }
                resetEvent.Set();
            }
            else
            {
                connected = false;
                lock (sync)
                {
                    Monitor.PulseAll(sync);
                }
            }
        }
    }
}