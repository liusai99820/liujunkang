using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.Redis;

namespace UnitTestProject
{
    [TestClass]
    public class TestClusterSortedSet
    {
        private static ClusterClient client;

        [ClassInitialize]
        public static void Start(TestContext testContext)
        {
            testContext.WriteLine("开始对类ClusterSortedSet执行单元测试......");
            client = new ClusterClient();
        }
        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        [TestMethod]
        public void TestSortedSetAdd()
        {
            Dictionary<int, string> data = new Dictionary<int, string>
            {
                { 1,"a"},
                { 2,"b"},
                { 3,"c"},
                { 4,"a"},
                { 5,"b"},
                { 6,"c"}
            };
            int ren = client.SortedSetAdd("A_23", data);
            Assert.AreEqual(3, ren);
            bool isok = client.StringDel("A_23");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 返回有序集合中元素的数量
        /// </summary>
        [TestMethod]
        public void TestSortedSetZcard()
        {
            Dictionary<int, string> data = new Dictionary<int, string>
            {
                { 1,"a"},
                { 2,"b"},
                { 3,"c"},
                { 4,"a"},
                { 5,"b"},
                { 6,"c"}
            };
            client.SortedSetAdd("A_24", data);
            int ren = client.SortedSetZcard("A_24");
            Assert.AreEqual(3, ren);
            bool isok = client.StringDel("A_24");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数
        /// </summary>
        [TestMethod]
        public void TestSortedSetZcount()
        {
            Dictionary<int, string> data = new Dictionary<int, string>
            {
                { 100,"张三"},
                { 99,"李四"},
                { 70,"王五"},
                { 60,"Jack"},
                { 50,"Bob"},
                { 59,"Lucy"}
            };
            client.SortedSetAdd("A_25", data);
            //获取考试成绩及格的人数
            int ren = client.SortedSetZcount("A_25", 60, 100);
            Assert.AreEqual(4, ren);
            bool isok = client.StringDel("A_25");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 返回指定区间内有序集合的元素
        /// </summary>
        [TestMethod]
        public void TestSortedSetZRange()
        {
            Dictionary<int, string> data = new Dictionary<int, string>
            {
                { 100,"张三"},
                { 99,"李四"},
                { 70,"王五"},
                { 60,"Jack"},
                { 50,"Bob"},
                { 59,"Lucy"}
            };
            client.SortedSetAdd("A_27", data);
            var list = client.SortedSetZRange("A_27", 0, 2);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "Lucy"));
            Assert.AreEqual(true, list.Exists(p => p == "Bob"));
            Assert.AreEqual(true, list.Exists(p => p == "Lucy"));

            list = client.SortedSetZRange("A_27", 3, 5);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "张三"));
            Assert.AreEqual(true, list.Exists(p => p == "李四"));
            Assert.AreEqual(true, list.Exists(p => p == "王五"));
            bool isok = client.StringDel("A_27");
            Assert.IsTrue(isok);
        }
        /// <summary>
        /// 返回指定区间内有序集合的元素，带分数
        /// </summary>
        [TestMethod]
        public void TestSortedSetZRangeWithscores()
        {
            Dictionary<int, string> data = new Dictionary<int, string>
            {
                { 100,"张三"},
                { 99,"李四"},
                { 70,"王五"},
                { 60,"Jack"},
                { 50,"Bob"},
                { 59,"Lucy"}
            };
            client.SortedSetAdd("A_28", data);
            var dic = client.SortedSetZRangeWithscores("A_28", 0, 2);
            Assert.AreEqual(3, dic.Count);
            Assert.AreEqual(true, dic.Keys.ToList().Exists(p => p == "Jack"));
            Assert.AreEqual(true, dic.Keys.ToList().Exists(p => p == "Bob"));
            Assert.AreEqual(true, dic.Keys.ToList().Exists(p => p == "Lucy"));
            dic = client.SortedSetZRangeWithscores("A_28", 3, 5);
            Assert.AreEqual(3, dic.Count);
            Assert.AreEqual(true, dic.Keys.ToList().Exists(p => p == "张三"));
            Assert.AreEqual(true, dic.Keys.ToList().Exists(p => p == "李四"));
            Assert.AreEqual(true, dic.Keys.ToList().Exists(p => p == "王五"));
            bool isok = client.StringDel("A_28");
            Assert.IsTrue(isok);
        }
        /// <summary>
        /// 返回元素在有序集合中的排名(从小到大)
        /// </summary>
        [TestMethod]
        public void TestSortedSetZRank()
        {
            Dictionary<int, string> data = new Dictionary<int, string>
            {
                { 100,"张三"},
                { 99,"李四"},
                { 70,"王五"},
                { 60,"Jack"},
                { 50,"Bob"},
                { 59,"Lucy"}
            };
            //注意，序号是从零开始算的
            client.SortedSetAdd("A_29", data);
            int rank = client.SortedSetZRank("A_29", "Lucy");
            Assert.AreEqual(1, rank);
            rank = client.SortedSetZRank("A_29", "王五");
            Assert.AreEqual(3, rank);
            rank = client.SortedSetZRank("A_29", "张三");
            Assert.AreEqual(5, rank);
            bool isok = client.StringDel("A_29");
            Assert.IsTrue(isok);
        }
        /// <summary>
        /// 返回元素在有序集合中的排名(从大到小)
        /// </summary>
        [TestMethod]
        public void TestSortedSetZRevRank()
        {
            Dictionary<int, string> data = new Dictionary<int, string>
            {
                { 100,"张三"},
                { 99,"李四"},
                { 70,"王五"},
                { 60,"Jack"},
                { 50,"Bob"},
                { 59,"Lucy"}
            };
            //注意，序号是从零开始算的
            client.SortedSetAdd("A_30", data);
            int rank = client.SortedSetZRevRank("A_30", "Lucy");
            Assert.AreEqual(4, rank);
            rank = client.SortedSetZRevRank("A_30", "王五");
            Assert.AreEqual(2, rank);
            rank = client.SortedSetZRevRank("A_30", "张三");
            Assert.AreEqual(0, rank);
            bool isok = client.StringDel("A_30");
            Assert.IsTrue(isok);
        }
        /// <summary>
        /// 移除有序集合中的一个或多个元素
        /// </summary>
        [TestMethod]
        public void TestSortedSetRem()
        {
            Dictionary<int, string> data = new Dictionary<int, string>
            {
                { 100,"张三"},
                { 99,"李四"},
                { 70,"王五"},
                { 60,"Jack"},
                { 50,"Bob"},
                { 59,"Lucy"}
            };
            client.SortedSetAdd("A_31", data);
            int ren = client.SortedSetRem("A_31", new string[] { "张三" });
            Assert.AreEqual(1, ren);
            ren = client.SortedSetRem("A_31", new string[] { "张三" });
            Assert.AreEqual(0, ren);
            ren = client.SortedSetRem("A_31", new string[] { "Bob", "Lucy" });
            Assert.AreEqual(2, ren);
            var list = client.SortedSetZRange("A_31", 0, 10);
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list.Exists(p => p == "李四"));
            Assert.IsTrue(list.Exists(p => p == "王五"));
            Assert.IsTrue(list.Exists(p => p == "Jack"));
            bool isok = client.StringDel("A_31");
            Assert.IsTrue(isok);
        }
        /// <summary>
        /// 获取有序集合指定元素的分数
        /// </summary>
        [TestMethod]
        public void TestSortedSetZScore()
        {
            Dictionary<int, string> data = new Dictionary<int, string>
            {
                { 100,"张三"},
                { 99,"李四"},
                { 70,"王五"},
                { 60,"Jack"},
                { 50,"Bob"},
                { 59,"Lucy"}
            };
            client.SortedSetAdd("A_32", data);
            var value = client.SortedSetZScore("A_32", "张三");
            Assert.AreEqual(value, "100");
            value = client.SortedSetZScore("A_32", "Jack");
            Assert.AreEqual(value, "60");
            value = client.SortedSetZScore("A_32", "Lucy");
            Assert.AreEqual(value, "59");
            bool isok = client.StringDel("A_32");
            Assert.IsTrue(isok);
        }
        /// <summary>
        /// 根据模式检索满足条件的元素
        /// </summary>
        [TestMethod]
        public void TestSortedSetScan()
        {
            Dictionary<int, string> data = new Dictionary<int, string>
            {
                { 100,"张三"},
                { 99,"李四"},
                { 67,"李广"},
                { 50,"李梅"}
            };
            client.SortedSetAdd("A_35", data);
            var dic = client.SortedSetScan("A_35", "李*");
            Assert.AreEqual(3, dic.Count);
            Assert.IsTrue(dic.Keys.Contains("李四"));
            Assert.IsTrue(dic.Keys.Contains("李广"));
            Assert.IsTrue(dic.Keys.Contains("李梅"));
            bool isok = client.StringDel("A_35");
            Assert.IsTrue(isok);
        }
    }
}
