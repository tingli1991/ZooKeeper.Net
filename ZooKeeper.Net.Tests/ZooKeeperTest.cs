using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZooKeeper.Net.Tests
{
    [TestClass]
    public class ZooKeeperTest
    {
        /// <summary>
        /// 初始化测试用例
        /// </summary>
        [TestMethod]
        public void TestInit()
        {
            var connectstring = "127.0.0.1:2181";
            var sessionTimeout = new TimeSpan(0, 0, 0, 10);
            using (var zk = new ZooKeeper(connectstring, sessionTimeout, new ConnectWatcher()))
            {
                string name = "/" + Guid.NewGuid().ToString().ToUpper() + "acltest";
                var result = zk.Create(name, new byte[0], Ids.CREATOR_ALL_ACL, CreateMode.Persistent);
            }
        }
    }
}