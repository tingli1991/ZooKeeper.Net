using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientConnectionEventConsumer : IStartable, IDisposable
    {
        private readonly ClientConnection conn;
        private readonly Thread eventThread;
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientConnectionEventConsumer));
        internal readonly BlockingCollection<ClientConnection.WatcherSetEventPair> waitingEvents = new BlockingCollection<ClientConnection.WatcherSetEventPair>();

        /** This is really the queued session state until the event
         * thread actually processes the event and hands it to the watcher.
         * But for all intents and purposes this is the state.
         */
        private volatile KeeperState sessionState = KeeperState.Disconnected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        public ClientConnectionEventConsumer(ClientConnection conn)
        {
            this.conn = conn;
            eventThread = new Thread(new SafeThreadStart(PollEvents).Run) { Name = new StringBuilder("ZK-EventThread ").Append(conn.zooKeeper.Id).ToString(), IsBackground = true };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            eventThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="watchers"></param>
        /// <param name="watchedEvent"></param>
        private static void ProcessWatcher(IEnumerable<IWatcher> watchers, WatchedEvent watchedEvent)
        {
            foreach (IWatcher watcher in watchers)
            {
                try
                {
                    if (null != watcher)
                    {
                        watcher.Process(watchedEvent);
                    }
                }
                catch (Exception t)
                {
                    log.Error("Error while calling watcher ", t);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PollEvents()
        {
            try
            {
                while (!waitingEvents.IsCompleted)
                {
                    try
                    {
                        ClientConnection.WatcherSetEventPair pair = null;
                        if (waitingEvents.TryTake(out pair, -1))
                        {
                            ProcessWatcher(pair.Watchers, pair.WatchedEvent);
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                        //ignored
                    }
                    catch (Exception t)
                    {
                        log.Error("Caught unexpected throwable", t);
                    }
                }
            }
            catch (ThreadInterruptedException e)
            {
                log.Error("Event thread exiting due to interruption", e);
            }
            log.Info("EventThread shut down");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        public void QueueEvent(WatchedEvent @event)
        {
            if (@event.Type == EventType.None && sessionState == @event.State)
                return;

            if (waitingEvents.IsAddingCompleted)
                throw new InvalidOperationException("consumer has been disposed");

            sessionState = @event.State;

            // materialize the watchers based on the event
            var pair = new ClientConnection.WatcherSetEventPair(conn.watcher.Materialize(@event.State, @event.Type, @event.Path), @event);
            // queue the pair (watch set & event) for later processing
            waitingEvents.Add(pair);
        }

        /// <summary>
        /// 
        /// </summary>
        private int isDisposed = 0;


        /// <summary>
        /// 
        /// </summary>
        private void InternalDispose()
        {
            if (Interlocked.CompareExchange(ref isDisposed, 1, 0) == 0)
            {
                try
                {
                    waitingEvents.CompleteAdding();

                    if (eventThread.IsAlive)
                    {
                        eventThread.Join();
                    }
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
        public void Dispose()
        {
            InternalDispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        ~ClientConnectionEventConsumer()
        {
            InternalDispose();
        }
    }
}