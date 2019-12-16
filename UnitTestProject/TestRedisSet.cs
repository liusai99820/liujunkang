using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.Redis;

namespace UnitTestProject
{
    [TestClass]
    public class TestRedisSet
    {
        private static RedisClient client;

        [ClassInitialize]
        public static void Start(TestContext testContext)
        {
            testContext.WriteLine("开始对类RedisSet执行单元测试......");
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
        /// 返回多个集合的差集
        /// </summary>
        [TestMethod]
        public void TestSetDiff()
        {
            client.SetAdd("A_11", new string[] { "tt", "gg" });
            client.SetAdd("B_14", new string[] { "tt1", "gg1" });
            var list = client.SetDiff(new string[] { "A_11", "B_14" });
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "tt"));
            Assert.AreEqual(true, list.Exists(p => p == "gg"));

            client.SetAdd("C_5", new string[] { "tt", "gg", "bb" });
            list = client.SetDiff(new string[] { "A_11", "C_5" });
            Assert.AreEqual(0, list.Count);

            list = client.SetDiff(new string[] { "C_5", "A_11" });
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "bb"));

            list = client.SetDiff(new string[] { "B_14", "C_5" });
            Assert.AreEqual(2, list.Count);

            list = client.SetDiff(new string[] { "A_11", "B_14", "C_5" });
            Assert.AreEqual(0, list.Count);
            bool isok = client.StringDel(new string[] { "A_11", "B_14", "C_5" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 返回多个集合的差集并存储在destination中
        /// </summary>
        [TestMethod]
        public void TestSetDiffStore()
        {
            client.SetAdd("A_12", new string[] { "tt", "gg" });
            client.SetAdd("B_15", new string[] { "tt1", "gg1" });
            client.SetDiffStore("A1", new string[] { "A_12", "B_15" });

            var list = client.SetMembers("A1");
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "tt"));
            Assert.AreEqual(true, list.Exists(p => p == "gg"));

            client.SetAdd("C_6", new string[] { "tt", "gg", "bb" });
            client.SetDiffStore("A2", new string[] { "A_12", "C_6" });
            list = client.SetMembers("A2");
            Assert.AreEqual(0, list.Count);

            client.SetDiffStore("A3",new string[] { "C_6", "A_12" });
            list = client.SetMembers("A3");
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "bb"));

            client.SetDiffStore("A4",new string[] { "B_15", "C_6" });
            list = client.SetMembers("A4");
            Assert.AreEqual(2, list.Count);

            client.SetDiffStore("A5",new string[] { "A_12", "B_15", "C_6" });
            list = client.SetMembers("A5");
            Assert.AreEqual(0, list.Count);
            bool isok = client.StringDel(new string[] { "A_12", "B_15", "C_6" ,"A1","A2","A3","A4","A5"});
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 返回多个集合间的交集
        /// </summary>
        [TestMethod]
        public void TestSetInter()
        {
            client.SetAdd("A_13", new string[] { "tt", "gg" });
            client.SetAdd("B_16", new string[] { "tt1", "gg1" });
            var list = client.SetInter(new string[] { "A", "B" });
            Assert.AreEqual(0, list.Count);

            client.SetAdd("C_7", new string[] { "tt", "gg", "bb" });
            list = client.SetInter(new string[] { "A_13", "C_7" });
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "tt"));
            Assert.AreEqual(true, list.Exists(p => p == "gg"));

            bool isok = client.StringDel(new string[] { "A_13", "B_16", "C_7" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 返回多个集合间的交集并存储在destination中
        /// </summary>
        [TestMethod]
        public void TestSetInterStore()
        {
            client.SetAdd("A_14", new string[] { "tt", "gg" });
            client.SetAdd("B_16", new string[] { "tt1", "gg1" });
            client.SetInterStore("A1",new string[] { "A", "B" });
            var list = client.SetMembers("A1");
            Assert.AreEqual(0, list.Count);

            client.SetAdd("C_8", new string[] { "tt", "gg", "bb" });
            client.SetInterStore("A2", new string[] { "A_14", "C_8" });
            list = client.SetMembers("A2");
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "tt"));
            Assert.AreEqual(true, list.Exists(p => p == "gg"));

            bool isok = client.StringDel(new string[] { "A_14", "B_16", "C_8", "A1", "A2" });
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
        /// 将元素value从from移动到to
        /// </summary>
        [TestMethod]
        public void TestSetMove()
        {
            client.SetAdd("A_17", new string[] { "tt", "gg" });
            client.SetAdd("B_17", new string[] { "tt2", "gg2" });
            client.SetMove("A_17", "B_17", "gg");
            var list1 = client.SetMembers("A_17");
            Assert.AreEqual(1, list1.Count);
            Assert.AreEqual(false, list1.Exists(p => p == "gg"));

            list1 = client.SetMembers("B_17");
            Assert.AreEqual(3, list1.Count);
            Assert.AreEqual(true, list1.Exists(p => p == "gg"));

            bool isok = client.StringDel(new string[] { "A_17", "B_17" });
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
            Assert.AreEqual(2, client.SetRem("A_19", new string[] { "tt","gg" }));
            var list = client.SetMembers("A_19");
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(false, list.Exists(p => p == "tt"));
            Assert.AreEqual(false, list.Exists(p => p == "gg"));
            bool isok = client.StringDel("A_19");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 返回多个集合的并集
        /// </summary>
        [TestMethod]
        public void TestSetUnion()
        {
            client.SetAdd("A_20", new string[] { "tt", "gg" });
            client.SetAdd("B_20", new string[] { "tt2", "gg2" });
            var list = client.SetUnion(new string[] { "A_20", "B_20" });
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "tt"));
            Assert.AreEqual(true, list.Exists(p => p == "gg"));
            Assert.AreEqual(true, list.Exists(p => p == "tt2"));
            Assert.AreEqual(true, list.Exists(p => p == "gg2"));
            bool isok = client.StringDel(new string[] { "A_20", "B_20" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 返回多个集合的并集并存储在destination中
        /// </summary>
        [TestMethod]
        public void TestSetUnionStore()
        {
            client.SetAdd("A_21", new string[] { "tt", "gg" });
            client.SetAdd("B_21", new string[] { "tt2", "gg2" });
            client.SetUnionStore("A1_9", new string[] { "A_21", "B_21" });
            var list = client.SetMembers("A1_9");
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "tt"));
            Assert.AreEqual(true, list.Exists(p => p == "gg"));
            Assert.AreEqual(true, list.Exists(p => p == "tt2"));
            Assert.AreEqual(true, list.Exists(p => p == "gg2"));
            bool isok = client.StringDel(new string[] { "A_21", "B_21", "A1_9" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 根据模式检索满足条件的元素
        /// </summary>
        [TestMethod]
        public void TestSetScan()
        {
            client.SetAdd("A_22", new string[] { "tt", "gg","劳动","劳改犯" });
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
