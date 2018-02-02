using System;
using Org.Apache.Zookeeper.Proto;
using System.Text;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class WatchedEvent
    {
        private readonly KeeperState state;
        private readonly EventType type;
        private readonly string path;

        public WatchedEvent(KeeperState state, EventType type, string path)
        {
            this.state = state;
            this.type = type;
            this.path = path;
        }

        public WatchedEvent(WatcherEvent eventMessage)
        {
            state = (KeeperState)Enum.ToObject(typeof(KeeperState), eventMessage.State);
            type = (EventType)Enum.ToObject(typeof(EventType), eventMessage.Type);
            path = eventMessage.Path;
        }

        public KeeperState State
        {
            get { return state; }
        }

        public EventType Type
        {
            get { return type; }
        }

        public string Path
        {
            get { return path; }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("WatchedEvent state:")
            .Append(state)
            .Append(" type:").Append(type)
            .Append(" path:").Append(path);
            return builder.ToString();
        }

        /**
         *  Convert WatchedEvent to type that can be sent over network
         */
        public WatcherEvent Wrapper
        {
            get
            {
                return new WatcherEvent((int)type, (int)state, path);
            }
        }
    }
}