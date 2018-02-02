using System;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class States : IEquatable<States>
    {
        public static readonly States CONNECTING = new States("CONNECTING");
        public static readonly States ASSOCIATING = new States("ASSOCIATING");
        public static readonly States CONNECTED = new States("CONNECTED");
        public static readonly States CLOSED = new States("CLOSED");
        public static readonly States AUTH_FAILED = new States("AUTH_FAILED");
        public static readonly States NOT_CONNECTED = new States("NOT_CONNECTED");

        /// <summary>
        /// 
        /// </summary>
        private string state;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public States(string state)
        {
            this.state = state;
        }

        /// <summary>
        /// 
        /// </summary>
        public string State
        {
            get { return state; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return this != CLOSED && this != AUTH_FAILED;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(States other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.state, state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((States)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (state != null ? state.GetHashCode() : 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return state;
        }
    }
}