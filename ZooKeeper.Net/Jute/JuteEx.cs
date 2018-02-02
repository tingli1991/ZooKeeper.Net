namespace Org.Apache.Jute
{
    /// <summary>
    /// 
    /// </summary>
    public static class JuteEx
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static int CompareTo(this byte[] b1, byte[] b2)
        {
            if (b1 == null && b2 == null) return 0;
            if (b1 == null || b2 == null) return 0;

            for (var i = 0; i < b1.Length; i++)
            {
                for (var j = 0; j < b2.Length; j++)
                {
                    if (i > j) return 1;
                    if (i < j) return -1;
                }
            }
            return 0;
        }
    }
}