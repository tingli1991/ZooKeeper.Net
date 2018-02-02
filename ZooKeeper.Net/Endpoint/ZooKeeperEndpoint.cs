using System;
using System.Net;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class ZooKeeperEndpoint
    {
        private const int retryCeiling = 10;
        private readonly IPEndPoint serverAddress;
        private DateTime nextAvailability = DateTime.MinValue;
        private int retryAttempts = 0;
        private TimeSpan backoffInterval;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="backoffInterval"></param>
        public ZooKeeperEndpoint(IPEndPoint serverAddress, TimeSpan backoffInterval)
        {
            this.serverAddress = serverAddress;
            this.backoffInterval = backoffInterval;
        }

        public IPEndPoint ServerAddress
        {
            get { return this.serverAddress; }
        }

        public DateTime NextAvailability
        {
            get { return this.nextAvailability; }
        }

        public int RetryAttempts
        {
            get { return this.retryAttempts; }
        }

        public void SetAsSuccess()
        {
            this.retryAttempts = 0;
            this.nextAvailability = DateTime.MinValue;
        }

        public void SetAsFailure()
        {
            this.retryAttempts++;
            this.nextAvailability = this.nextAvailability == DateTime.MinValue ? DateTime.UtcNow : this.nextAvailability;
            this.nextAvailability = GetNextAvailability(this.nextAvailability, this.backoffInterval, this.retryAttempts);
        }

        public static DateTime GetNextAvailability(DateTime currentAvailability, TimeSpan backoffInterval, int retryAttempts)
        {
            double backoffIntervalMinutes = backoffInterval.Minutes >= 1 ? backoffInterval.Minutes : 1;
            int backoffMinutes = (int)((1d / backoffIntervalMinutes) * (Math.Pow(backoffIntervalMinutes, Math.Min(retryAttempts, retryCeiling)) - 1d));
            backoffMinutes = backoffMinutes >= 1 ? backoffMinutes : 1;
            return currentAvailability.AddMinutes(backoffMinutes);
        }
    }
}