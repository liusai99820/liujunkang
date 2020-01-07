using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.Redis;

namespace UnitTestProject
{
    [TestClass]
    public class TestClusterSet
    {
        private static ClusterClient client;

        [ClassInitialize]
        public static void Start(TestContext testContext)
        {
            testContext.WriteLine("开始对类ClusterSet执行单元测试......");
            client = new ClusterClient();
        }

        /// <summary>
        /// 向集合中添加一个或多个成员
        /// </summary>
        [TestMethod]
        public void TestSetAdd()
        {
            int rlen = client.SetAdd("A_9", new string[] { "1", "2", "我们", "《》", "2" });
            Assert.AreEqual(4, rlen, "set集合里面不允许出现重复的数据");
            var list = client.SetMembers("A_9");
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "1"));
            Assert.AreEqual(true, list.Exists(p => p == "2"));
            Assert.AreEqual(true, list.Exists(p => p == "我们"));
            Assert.AreEqual(true, list.Exists(p => p == "《》"));
            bool isok = client.StringDel("A_9");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 获取集合的成员个数
        /// </summary>
        [TestMethod]
        public void TestSetCard()
        {
            client.SetAdd("A_10", new string[] { "1", "2", "我们", "《》", "2" });
            int rlen = client.SetCard("A_10");
            Assert.AreEqual(4, rlen);

            client.SetAdd("A_10", new string[] { "1", "2", "我们", "《》", "2" });
            rlen = client.SetCard("A_10");
            Assert.AreEqual(4, rlen);

            client.SetAdd("A_10", new string[] { "tt", "gg" });
            rlen = client.SetCard("A_10");
            Assert.AreEqual(6, rlen);
            bool isok = client.StringDel("A_10");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 判断成员元素是否是集合的成员
        /// </summary>
        [TestMethod]
        public void TestSetSisMember()
        {
            client.SetAdd("A_15", new string[] { "tt", "gg" });
            Assert.AreEqual(true, client.SetSisMember("A_15", "tt"));
            Assert.AreEqual(true, client.SetSisMember("A_15", "tt"));
            Assert.AreEqual(false, client.SetSisMember("A_15", "TT"));
            bool isok = client.StringDel("A_15");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 返回集合key中所有的成员
        /// </summary>
        [TestMethod]
        public void TestSetMembers()
        {
            client.SetAdd("A_16", new string[] { "tt", "gg" });
            var list = client.SetMembers("A_16");
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "tt"));
            Assert.AreEqual(true, list.Exists(p => p == "gg"));
            bool isok = client.StringDel("A_16");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 随机移除指定数量的元素并返回
        /// </summary>
        [TestMethod]
        public void TestSetPop()
        {
            client.SetAdd("A_18", new string[] { "tt", "gg", "1", "2", "3", "5" });
            var list = client.SetPop("A_18", 4);
            Assert.AreEqual(4, list.Count);
            list = client.SetMembers("A_18");
            Assert.AreEqual(2, list.Count);
            bool isok = client.StringDel("A_18");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 移除集合中的一个或多个元素
        /// </summary>
        [TestMethod]
        public void TestSetRem()
        {
            client.SetAdd("A_19", new string[] { "tt", "gg", "1", "2", "3", "5" });
            Assert.AreEqual(2, client.SetRem("A_19", new string[] { "tt", "gg" }));
            var list = client.SetMembers("A_19");
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(false, list.Exists(p => p == "tt"));
            Assert.AreEqual(false, list.Exists(p => p == "gg"));
            bool isok = client.StringDel("A_19");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 根据模式检索满足条件的元素
        /// </summary>
        [TestMethod]
        public void TestSetScan()
        {
            client.SetAdd("A_22", new string[] { "tt", "gg", "劳动", "劳改犯" });
            //查询劳字开头的
            List<string> dic = client.SetScan("A_22", "劳*");
            Assert.AreEqual(2, dic.Count);
            Assert.AreEqual(true, dic.Exists(p => p == "劳动"));
            Assert.AreEqual(true, dic.Exists(p => p == "劳改犯"));

            dic = client.SetScan("A_22", "ququ");
            Assert.AreEqual(0, dic.Count);

            bool isok = client.StringDel("A_22");
            Assert.AreEqual(true, isok);
        }
    }
}
