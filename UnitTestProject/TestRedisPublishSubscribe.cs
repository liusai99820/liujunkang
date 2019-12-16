using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.Redis;

namespace UnitTestProject
{
    [TestClass]
    public class TestRedisPublishSubscribe
    {
        private static RedisClient client;
        private delegate string mock(string channelName);

        [ClassInitialize]
        public static void Start(TestContext testContext)
        {
            testContext.WriteLine("开始对类RedisPublishSubscribe执行单元测试......");
            //注意，一定要选择一个空的数据库作为测试用例
            client = new RedisClient(15);
            client.Open();
        }

        [ClassCleanup]
        public static void End()
        {
            client.Dispose();
        }

        /// <summary>
        /// 订阅频道
        /// </summary>
        [TestMethod]
        public void TestSubscribeChannel()
        {
            mock _mock = Subscribe;
            _mock.BeginInvoke("testChannel", delegate (IAsyncResult r)
            {
                mock da2 = (mock)r.AsyncState;
                string msg = da2.EndInvoke(r);
                Assert.AreEqual("HelloWorld", msg);
            }, _mock);
        }
        private string Subscribe(string channelName)
        {
            using (RedisClient db = new RedisClient(15))
            {
                db.Open();
                string msg = db.SubscribeChannel(channelName);
                return msg;
            }
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        [TestMethod]
        public void TestPublishMessage()
        {
            client.PublishMessage("testChannel", "HelloWorld");
            Assert.IsTrue(true);
        }
    }
}
