using System;
using System.Collections.Generic;
using System.Net;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class ZooKeeperEndpoints : IEnumerable<ZooKeeperEndpoint>
    {
        private static readonly TimeSpan defaultBackoffInterval = new TimeSpan(0, 2, 0);
        private List<ZooKeeperEndpoint> zkEndpoints = new List<ZooKeeperEndpoint>();
        private ZooKeeperEndpoint endpoint = null;
        private int endpointID = -1;
        private bool isNextEndPointAvailable = true;
        private TimeSpan backoffInterval;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoints"></param>
        public ZooKeeperEndpoints(List<IPEndPoint> endpoints)
            : this(endpoints, defaultBackoffInterval)
        {
        }

        public ZooKeeperEndpoints(List<IPEndPoint> endpoints, TimeSpan backoffInterval)
        {
            this.backoffInterval = backoffInterval;
            AddRange(endpoints);
        }

        public IEnumerator<ZooKeeperEndpoint> GetEnumerator()
        {
            return this.zkEndpoints.GetEnumerator();
        }

        public void Add(IPEndPoint endPoint)
        {
            this.Add(new ZooKeeperEndpoint(endPoint, backoffInterval));
        }

        public void Add(ZooKeeperEndpoint endPoint)
        {
            zkEndpoints.Add(endPoint);
        }

        public void AddRange(List<ZooKeeperEndpoint> endPoints)
        {
            this.zkEndpoints.AddRange(endPoints);
        }

        public void AddRange(List<IPEndPoint> endPoints)
        {
            AddRange(endPoints.ConvertAll(e => new ZooKeeperEndpoint(e, backoffInterval)));
        }

        public ZooKeeperEndpoint CurrentEndPoint
        {
            get { return this.endpoint; }
        }

        public int EndPointID
        {
            get { return this.endpointID; }
        }

        public bool IsNextEndPointAvailable
        {
            get { return this.isNextEndPointAvailable; }
        }

        public void GetNextAvailableEndpoint()
        {
            isNextEndPointAvailable = true;
            if (this.zkEndpoints.Count > 0)
            {
                int startingPosition = endpointID;
                do
                {
                    endpointID++;
                    if (endpointID == zkEndpoints.Count)
                    {
                        endpointID = 0;
                    }

                    if (endpointID == startingPosition)
                    {
                        //All connections are disabled.
                        //Try one at a time until success
                        ResetConnections(endpointID);
                    }
                }
                while (this.zkEndpoints[endpointID].NextAvailability > DateTime.UtcNow);
            }

            this.endpoint = zkEndpoints[endpointID];
        }

        private void ResetConnections(int currentPostition)
        {
            for (int i = 0; i < zkEndpoints.Count; i++)
            {
                if (zkEndpoints[i].RetryAttempts > 1)
                {
                    //clear connection
                    zkEndpoints[i].SetAsSuccess();
                    //set back to lowest backoff
                    zkEndpoints[i].SetAsFailure();
                }
            }

            //Enable current connection
            zkEndpoints[endpointID].SetAsSuccess();
            isNextEndPointAvailable = false;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}