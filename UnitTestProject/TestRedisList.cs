using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.Redis;

namespace UnitTestProject
{
    [TestClass]
    public class TestRedisList
    {
        private static RedisClient client;

        [ClassInitialize]
        public static void Start(TestContext testContext)
        {
            testContext.WriteLine("开始对类RedisString执行单元测试......");
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
        /// 将一个或多个值插入列表头部，如果key不存在则会创建一个空列表后执行push操作
        /// </summary>
        [TestMethod]
        public void TestListLPush()
        {
            string[] values = { "32","ABD","大象","@###？"};
            int rlen = client.ListLPush("B_1", values);
            Assert.AreEqual(true, rlen > 0);
            List<string> data = client.ListLRange("B_1", 0, 10);
            Assert.AreEqual(4, data.Count);
            Assert.AreEqual("32", data[3]);
            Assert.AreEqual("ABD", data[2]);
            Assert.AreEqual("大象", data[1]);
            Assert.AreEqual("@###？", data[0]);

            client.StringSet("A_4", "test");
            rlen = client.ListLPush("A_4", values);
            Assert.AreEqual(0, rlen);

            bool isok = client.StringDel(new string[] { "B_1", "A_4" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 将一个或多个值插入列表尾部，如果key不存在则会创建一个空列表后执行push操作
        /// </summary>
        [TestMethod]
        public void TestListRPush()
        {
            string[] values = { "32", "ABD", "大象", "@###？" };
            int rlen = client.ListRPush("B_2", values);
            Assert.AreEqual(true, rlen > 0);
            List<string> data = client.ListLRange("B_2", 0, 10);
            Assert.AreEqual(4, data.Count);
            Assert.AreEqual("32", data[0]);
            Assert.AreEqual("ABD", data[1]);
            Assert.AreEqual("大象", data[2]);
            Assert.AreEqual("@###？", data[3]);

            client.StringSet("A_5", "test");
            rlen = client.ListRPush("A_5", values);
            Assert.AreEqual(0, rlen);

            bool isok = client.StringDel(new string[] { "B_2", "A_5" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 将一个值插入已存在列表头部，并且返回列表长度。如果列表不存在则命令无效。只有当返回值大于零的时候才证明操作是成功的！
        /// </summary>
        [TestMethod]
        public void TestListLPushX()
        {
            string[] values = { "32", "ABD", "大象", "@###？" };
            client.ListLPush("B_3", values);
            int rlen = client.ListLPushX("B_3", "parliament");
            Assert.AreEqual(true, rlen > 0);

            rlen = client.ListLPushX("C_1", "personality");
            Assert.AreEqual(false, rlen > 0);

            bool isok = client.StringDel(new string[] { "B_3", "C_1" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 将一个值插入已存在列表尾部，并且返回列表长度。如果列表不存在则命令无效。只有当返回值大于零的时候才证明操作是成功的！
        /// </summary>
        [TestMethod]
        public void TestListRPushX()
        {
            string[] values = { "32", "ABD", "大象", "@###？" };
            client.ListRPush("B_4", values);
            int rlen = client.ListRPushX("B_4", "parliament");
            Assert.AreEqual(true, rlen > 0);

            rlen = client.ListRPushX("C_2", "personality");
            Assert.AreEqual(false, rlen > 0);

            bool isok = client.StringDel(new string[] { "B_4", "C_2" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 通过索引查找值
        /// </summary>
        [TestMethod]
        public void TestListLIndex()
        {
            string[] values = { "32", "ABD", "大象", "@###？" };
            client.ListRPush("B_5", values);
            Assert.AreEqual("32", client.ListLIndex("B_5", 0));
            Assert.AreEqual("ABD", client.ListLIndex("B_5", 1));
            Assert.AreEqual("大象", client.ListLIndex("B_5", 2));
            Assert.AreEqual("@###？", client.ListLIndex("B_5", 3));
            Assert.AreEqual(string.Empty, client.ListLIndex("B_5", 4));
            Assert.AreEqual(string.Empty, client.ListLIndex("D_1", 0));
            client.StringSet("A_6", "test");
            Assert.AreEqual(string.Empty, client.ListLIndex("A_6", 0));
            bool isok = client.StringDel(new string[] { "B_5", "A_6", "D_1" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 获取列表长度
        /// </summary>
        [TestMethod]
        public void TestListLlenKey()
        {
            string[] values = { "32", "ABD", "大象", "@###？" };
            client.ListRPush("B_6", values);
            Assert.AreEqual(4, client.ListLlenKey("B_6"));
            Assert.AreEqual(0, client.ListLlenKey("A_7"));
            client.StringSet("c", "test");
            Assert.AreEqual(0, client.ListLlenKey("c"));
            bool isok = client.StringDel(new string[] { "B_6", "A_7", "c" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 根据参数 COUNT 的值，移除列表中与参数 VALUE 相等的元素。count 大于 0 : 从表头开始向表尾搜索，移除与 VALUE 相等的元素，数量为 COUNT 。count 小于 0 : 从表尾开始向表头搜索，移除与 VALUE 相等的元素，数量为 COUNT 的绝对值。count 等于 0 : 移除表中所有与 VALUE 相等的值。
        /// </summary>
        [TestMethod]
        public void TestListLRem()
        {
            string[] values = { "32", "ABD", "大象", "@###？" ,"A","B","D","455","455","A"};
            client.ListRPush("B_7", values);
            int rem = client.ListLRem("B_7", 1, "32");
            Assert.AreEqual(1, rem);
            List<string> data = client.ListLRange("B_7", 0, 10);
            Assert.AreEqual(false, data.Exists(p => p == "32"));

            rem = client.ListLRem("B_7", -5, "455");
            Assert.AreEqual(2, rem);
            data = client.ListLRange("B_7", 0, 10);
            Assert.AreEqual(false, data.Exists(p => p == "455"));

            rem = client.ListLRem("B_7", -5, "D");
            Assert.AreEqual(1, rem);
            data = client.ListLRange("B_7", 0, 10);
            Assert.AreEqual(false, data.Exists(p => p == "D"));

            rem = client.ListLRem("B_7", 0, "A");
            Assert.AreEqual(2, rem);
            data = client.ListLRange("B_7", 0, 10);
            Assert.AreEqual(false, data.Exists(p => p == "A"));

            bool isok = client.StringDel("B_7");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 移除并获取列表的第一个元素
        /// </summary>
        [TestMethod]
        public void TestListLPop()
        {
            string[] values = { "32", "ABD", "大象", "@###？", "A", "B", "D", "455", "455", "A" };
            client.ListRPush("B_8", values);
            string topvalue = client.ListLPop("B_8");
            Assert.AreEqual("32", topvalue);
            topvalue = client.ListLPop("B_8");
            Assert.AreEqual("ABD", topvalue);
            topvalue = client.ListLPop("B_8");
            Assert.AreEqual("大象", topvalue);
            List<string> data = client.ListLRange("B_8", 0, 10);
            Assert.AreEqual(7, data.Count);
            Assert.AreEqual(false, data.Exists(p => p == "32"));
            Assert.AreEqual(false, data.Exists(p => p == "ABD"));
            Assert.AreEqual(false, data.Exists(p => p == "大象"));

            bool isok = client.StringDel("B_8");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 在pivot元素之前插入value元素。如果命令执行成功，返回插入操作完成之后，列表的长度。 如果没有找到指定元素 ，返回 -1 。 如果 key 不存在或为空列表，返回 0 。
        /// </summary>
        [TestMethod]
        public void TestListLInsertBefore()
        {
            string[] values = { "32", "ABD", "大象", "@###？", "A", "B", "D", "455", "455", "A" };
            client.ListRPush("B_9", values);
            int rlen = client.ListLInsertBefore("B_9", "大象", "美女");
            Assert.AreEqual(rlen, values.Length + 1);
            List<string> data = client.ListLRange("B_9", 0, 10);
            Assert.AreEqual(true, data.Exists(p => p == "美女"));
            Assert.AreEqual("美女", data[2]);
            Assert.AreEqual("大象", data[3]);

            rlen = client.ListLInsertBefore("B_9", "大象2", "美女");
            Assert.AreEqual(-1, rlen);

            rlen = client.ListLInsertBefore("b2", "大象2", "美女");
            Assert.AreEqual(0, rlen);

            bool isok = client.StringDel("B_9");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 在pivot元素之后插入value元素。如果命令执行成功，返回插入操作完成之后，列表的长度。 如果没有找到指定元素 ，返回 -1 。 如果 key 不存在或为空列表，返回 0 。
        /// </summary>
        [TestMethod]
        public void TestListLInsertAfter()
        {
            string[] values = { "32", "ABD", "大象", "@###？", "A", "B", "D", "455", "455", "A" };
            client.ListRPush("B_10", values);
            int rlen = client.ListLInsertAfter("B_10", "大象", "美女");
            Assert.AreEqual(rlen, values.Length + 1);
            List<string> data = client.ListLRange("B_10", 0, 10);
            Assert.AreEqual(true, data.Exists(p => p == "美女"));
            Assert.AreEqual("美女", data[3]);
            Assert.AreEqual("大象", data[2]);

            rlen = client.ListLInsertAfter("B_10", "大象2", "美女");
            Assert.AreEqual(-1, rlen);

            rlen = client.ListLInsertAfter("b2", "大象2", "美女");
            Assert.AreEqual(0, rlen);

            bool isok = client.StringDel("B_10");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 返回列表中指定区间内的元素
        /// </summary>
        [TestMethod]
        public void TestListLRange()
        {
            string[] values = { "32", "ABD", "大象", "@###？", "A", "B", "D", "455", "455", "A" };
            client.ListRPush("B_11", values);
            List<string> data = client.ListLRange("B_11", 0, 1);
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual("32", data[0]);
            Assert.AreEqual("ABD", data[1]);
            data = client.ListLRange("B_11", 4, 6);
            Assert.AreEqual(3, data.Count);
            Assert.AreEqual("A", data[0]);
            Assert.AreEqual("B", data[1]);
            Assert.AreEqual("D", data[2]);
            data = client.ListLRange("B_11", 0, 0);
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("32", data[0]);
            data = client.ListLRange("B_11", 0, -1);
            Assert.AreEqual(10, data.Count);
            data = client.ListLRange("B_11", 0, 11);
            Assert.AreEqual(10, data.Count);
            data = client.ListLRange("B_11", -5, -3);
            Assert.AreEqual(3, data.Count);
            data = client.ListLRange("B1", 0, 1);
            Assert.AreEqual(0, data.Count);
            client.StringSet("A_8", "test");
            data = client.ListLRange("A_8", 0, 1);
            Assert.AreEqual(0, data.Count);

            bool isok = client.StringDel(new string[] { "B_11", "A_8" });
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 移除并获取列表的最后一个元素
        /// </summary>
        [TestMethod]
        public void TestListRPop()
        {
            string[] values = { "32", "ABD", "大象", "@###？", "B", "D", "456", "455", "A" };
            client.ListRPush("B_12", values);
            string topvalue = client.ListRPop("B_12");
            Assert.AreEqual("A", topvalue);
            topvalue = client.ListRPop("B_12");
            Assert.AreEqual("455", topvalue);
            topvalue = client.ListRPop("B_12");
            Assert.AreEqual("456", topvalue);
            List<string> data = client.ListLRange("B_12", 0, 10);
            Assert.AreEqual(6, data.Count);
            Assert.AreEqual(false, data.Exists(p => p == "A"));
            Assert.AreEqual(false, data.Exists(p => p == "455"));
            Assert.AreEqual(false, data.Exists(p => p == "456"));
            bool isok = client.StringDel("B_12");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 列表只保留区间内的元素，不在区间内的元素都被删除
        /// </summary>
        [TestMethod]
        public void TestListLTrim()
        {
            string[] values = { "32", "ABD", "大象", "@###？", "B", "D", "456", "455", "A" };
            client.ListRPush("B_13", values);
            bool isok = client.ListLTrim("B_13", 3, 6);
            Assert.AreEqual(true, isok);
            List<string> data = client.ListLRange("B_13", 0, 10);
            Assert.AreEqual(4, data.Count);
            Assert.AreEqual(true, data.Exists(p => p == "@###？"));
            Assert.AreEqual(true, data.Exists(p => p == "B"));
            Assert.AreEqual(true, data.Exists(p => p == "D"));
            Assert.AreEqual(true, data.Exists(p => p == "456"));
            isok = client.StringDel("B_13");
            Assert.AreEqual(true, isok);
        }
    }
}
