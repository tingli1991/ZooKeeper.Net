namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CreateMode
    {
        public static readonly CreateMode Persistent = new CreateMode(0, false, false);
        public static readonly CreateMode PersistentSequential = new CreateMode(2, false, true);
        public static readonly CreateMode Ephemeral = new CreateMode(1, true, false);
        public static readonly CreateMode EphemeralSequential = new CreateMode(3, true, true);

        private readonly int flag;
        private readonly bool ephemeral;
        private readonly bool sequential;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="ephemeral"></param>
        /// <param name="sequential"></param>
        private CreateMode(int flag, bool ephemeral, bool sequential)
        {
            this.flag = flag;
            this.ephemeral = ephemeral;
            this.sequential = sequential;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Flag
        {
            get { return flag; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEphemeral
        {
            get { return ephemeral; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Sequential
        {
            get { return sequential; }
        }
    }
}