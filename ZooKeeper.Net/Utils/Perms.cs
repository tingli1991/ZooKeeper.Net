namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Perms
    {
        private Perms() { }
        public const int READ = 1 << 0;
        public const int WRITE = 1 << 1;
        public const int CREATE = 1 << 2;
        public const int DELETE = 1 << 3;
        public const int ADMIN = 1 << 4;
        public const int ALL = READ | WRITE | CREATE | DELETE | ADMIN;
    }
}