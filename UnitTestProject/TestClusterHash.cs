using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.Redis;

namespace UnitTestProject
{
    [TestClass]
    public class TestClusterHash
    {
        private static ClusterClient client;

        [ClassInitialize]
        public static void Start(TestContext testContext)
        {
            testContext.WriteLine("开始对类ClusterHash执行单元测试......");
            client = new ClusterClient();
        }
       
        /// <summary>
        /// 设置哈希值
        /// </summary>
        [TestMethod]
        public void TestHashSet()
        {
            bool isok = client.HashSet("A_1", "A1", "test");
            Assert.AreEqual(true, isok);
            string value = client.HashGet("A_1", "A1");
            Assert.AreEqual(value, "test");

            isok = client.HashSet("A_1", "A2", "中给");
            Assert.AreEqual(true, isok);
            value = client.HashGet("A_1", "A2");
            Assert.AreEqual(value, "中给");

            isok = client.HashSet("A_1", "A3", ",<>《。。》￥#$");
            Assert.AreEqual(true, isok);
            value = client.HashGet("A_1", "A3");
            Assert.AreEqual(value, ",<>《。。》￥#$");

            isok = client.StringDel("A_1");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 设置哈希值，注意只有在字段不存在的时候才能设置哈希值
        /// </summary>
        [TestMethod]
        public void TestHashSetNotExist()
        {
            bool isok = client.HashSetNotExist("A_2", "A1", "test");
            Assert.AreEqual(true, isok);
            string value = client.HashGet("A_2", "A1");
            Assert.AreEqual(value, "test");

            isok = client.HashSetNotExist("A_2", "A1", "test321321");
            Assert.AreEqual(false, isok);
            value = client.HashGet("A_2", "A1");
            Assert.AreEqual(value, "test");

            isok = client.HashSetNotExist("A_2", "A2", "中给");
            Assert.AreEqual(true, isok);
            value = client.HashGet("A_2", "A2");
            Assert.AreEqual(value, "中给");

            isok = client.HashSetNotExist("A_2", "A3", ",<>《。。》￥#$");
            Assert.AreEqual(true, isok);
            value = client.HashGet("A_2", "A3");
            Assert.AreEqual(value, ",<>《。。》￥#$");

            isok = client.StringDel("A_2");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 批量设置Hash值
        /// </summary>
        [TestMethod]
        public void TestHashMSet()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "A1","test"},
                { "A2","中给"},
                { "A3",",<>《。。》￥#$"},
            };
            bool isok = client.HashMSet("美国", data);
            Assert.AreEqual(true, isok);

            string value = client.HashGet("美国", "A1");
            Assert.AreEqual(value, "test");
            value = client.HashGet("美国", "A2");
            Assert.AreEqual(value, "中给");
            value = client.HashGet("美国", "A3");
            Assert.AreEqual(value, ",<>《。。》￥#$");

            isok = client.StringDel("美国");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 获取指定key和field的值
        /// </summary>
        [TestMethod]
        public void TestHashGet()
        {
            client.HashSet("A_3", "A1", "test");
            string value = client.HashGet("A_3", "A1");
            Assert.AreEqual(value, "test");

            client.HashSet("美国", "A1", "test");
            value = client.HashGet("美国", "A1");
            Assert.AreEqual(value, "test");

            client.HashSet("《》￥￥", "A1", "test");
            value = client.HashGet("《》￥￥", "A1");
            Assert.AreEqual(value, "test");

            client.HashSet("…………", "A1", "先帝创业未半，而中道崩殂");
            value = client.HashGet("…………", "A1");
            Assert.AreEqual(value, "先帝创业未半，而中道崩殂");

            client.StringDel("A_3");
            client.StringDel("美国");
            client.StringDel("《》￥￥");
            client.StringDel("…………");
        }
        /// <summary>
        /// 获取指定key下的所有字段
        /// </summary>
        [TestMethod]
        public void TestHashGetFields()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "A1","test"},
                { "A2","中给"},
                { "A3",",<>《。。》￥#$"},
            };
            client.HashMSet("美国", data);
            List<string> list = client.HashGetFields("美国");
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(true, list.Exists(p => p == "A1"));
            Assert.AreEqual(true, list.Exists(p => p == "A2"));
            Assert.AreEqual(true, list.Exists(p => p == "A3"));

            bool isok = client.StringDel("美国");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 批量获取指定字段的值,如果查不到值则略过
        /// </summary>
        [TestMethod]
        public void TestHashMGet()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "A1","test"},
                { "A2","中给"},
                { "A3",",<>《。。》￥#$"},
                { "A4","789"},
                { "西安","不倒翁"}
            };
            client.HashMSet("中国", data);
            Dictionary<string, string> values = client.HashMGet("中国", new string[] { "A1", "A2", "A4", "a2", "西安" });
            Assert.AreEqual(4, values.Count);
            Assert.AreEqual("test", values["A1"]);
            Assert.AreEqual("中给", values["A2"]);
            Assert.AreEqual("789", values["A4"]);
            Assert.AreEqual("不倒翁", values["西安"]);

            bool isok = client.StringDel("中国");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 获取指定key下所有的字段以及对应的值
        /// </summary>
        [TestMethod]
        public void TestHashMGetAll()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "A1","test"},
                { "A2","中给"},
                { "A3",",<>《。。》￥#$"},
                { "A4","789"},
                { "西安","不倒翁"}
            };
            client.HashMSet("中国", data);
            Dictionary<string, string> values = client.HashMGetAll("中国");
            Assert.AreEqual(5, values.Count);
            Assert.AreEqual("test", values["A1"]);
            Assert.AreEqual("中给", values["A2"]);
            Assert.AreEqual("789", values["A4"]);
            Assert.AreEqual("不倒翁", values["西安"]);
            Assert.AreEqual(",<>《。。》￥#$", values["A3"]);

            bool isok = client.StringDel("中国");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 查询key中指定的字段是否存在
        /// </summary>
        [TestMethod]
        public void TestHashExistField()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "A1","test"},
                { "A2","中给"},
                { "A3",",<>《。。》￥#$"},
                { "A4","789"},
                { "西安","不倒翁"}
            };
            client.HashMSet("中国", data);
            Assert.AreEqual(true, client.HashExistField("中国", "西安"));
            Assert.AreEqual(true, client.HashExistField("中国", "A4"));
            Assert.AreEqual(true, client.HashExistField("中国", "A3"));
            Assert.AreEqual(true, client.HashExistField("中国", "A2"));
            Assert.AreEqual(true, client.HashExistField("中国", "A1"));
            Assert.AreEqual(false, client.HashExistField("中国", "A6"));

            bool isok = client.StringDel("中国");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 删除一个或多个字段
        /// </summary>
        [TestMethod]
        public void TestHashDelField()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "A1","test"},
                { "A2","中给"},
                { "A3",",<>《。。》￥#$"},
                { "A4","789"},
                { "西安","不倒翁"}
            };
            client.HashMSet("中国", data);
            Assert.AreEqual(3, client.HashDelField("中国", new string[] { "西安", "A1", "A2" }));
            Assert.AreEqual(1, client.HashDelField("中国", new string[] { "西安2", "a1", "A3" }));
            Assert.AreEqual(true, client.HashExistField("中国", "A4"));
            Assert.AreEqual(false, client.HashExistField("中国", "西安"));
            Assert.AreEqual(false, client.HashExistField("中国", "A1"));
            Assert.AreEqual(false, client.HashExistField("中国", "A2"));

            bool isok = client.StringDel("中国");
            Assert.AreEqual(true, isok);
        }
    }
}
