using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.Redis;

namespace UnitTestProject
{
    [TestClass]
    public class TestRedisString
    {
        private static RedisClient client;

        [ClassInitialize]
        public static void  Start(TestContext testContext)
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
        /// 获取Key对应的Value
        /// </summary>
        [TestMethod]
        public void TestStringGet()
        {
            //准备数据
            client.StringSet("test0", "testvalue");
            client.StringSet("test1", "中国");
            client.StringSet("test2", "434324,,,，【】。。。。");
            client.StringSet("测试3", "哈哈");

            //执行测试
            Assert.AreEqual("testvalue", client.StringGet("test0"), "正常数据测试");
            Assert.AreEqual("中国", client.StringGet("test1"), "测试中文字符");
            Assert.AreEqual("434324,,,，【】。。。。", client.StringGet("test2"), "测试中文标点符号");
            Assert.AreEqual("哈哈", client.StringGet("测试3"), "测试中文键是否可以正常工作");

            //清理数据
            client.StringDel(new string[] { "test0", "test1", "test2", "测试3" });
        }
        /// <summary>
        /// 获取keys集合
        /// </summary>
        [TestMethod]
        public void TestStringKeys()
        {
            //准备数据
            client.StringSet("test0_1", "testvalue");
            client.StringSet("test1_1", "中国");
            client.StringSet("test2_1", "434324,,,，【】。。。。");
            client.StringSet("测试3_1", "哈哈");

            //执行测试
            List<string> keys = client.StringKeys("*");
            Assert.AreEqual(4, keys.Count, "总数是否一致");

            keys = client.StringKeys("test*");
            Assert.AreEqual(3, keys.Count, "总数是否一致");
            
            keys = client.StringKeys("测试3_1");
            Assert.AreEqual(1, keys.Count, "总数是否一致");
            Assert.AreEqual("测试3_1", keys[0], "内容是否一致");

            keys = client.StringKeys("test0_1");
            Assert.AreEqual(1, keys.Count, "总数是否一致");
            Assert.AreEqual("test0_1", keys[0], "内容是否一致");

            //清理数据
            client.StringDel(new string[] { "test0_1", "test1_1", "test2_1", "测试3_1" });
        }
        /// <summary>
        /// 判断指定的key是否存在
        /// </summary>
        [TestMethod]
        public void TestStringExists()
        {
            //准备数据
            client.StringSet("test0_2", "testvalue");
            client.StringSet("test1_2", "中国");
            client.StringSet("test2_2", "434324,,,，【】。。。。");
            client.StringSet("测试3_2", "哈哈");

            //执行测试
            Assert.AreEqual(true, client.StringExists("test0_2"));
            Assert.AreEqual(true, client.StringExists("test1_2"));
            Assert.AreEqual(true, client.StringExists("test2_2"));
            Assert.AreEqual(true, client.StringExists("测试3_2"));
            Assert.AreEqual(false, client.StringExists("测试4"));
            Assert.AreEqual(false, client.StringExists("test4"));
            Assert.AreEqual(false, client.StringExists("test"));

            //清理数据
            client.StringDel(new string[] { "test0_2", "test1_2", "test2_2", "测试3_2" });
        }
        /// <summary>
        /// 批量获取指定key的值,如果查不到值则略过
        /// </summary>
        [TestMethod]
        public void TestStringMGet()
        {
            //准备数据
            client.StringSet("test0_3", "testvalue");
            client.StringSet("test1_3", "中国");
            client.StringSet("test2_3", "434324,,,，【】。。。。");
            client.StringSet("测试3_3", "哈哈");

            //执行测试
            Dictionary<string, string> dic = client.StringMGet(new string[] { "test0_3", "test1_3", "test2_3", "测试3_3" });
            Assert.AreEqual(dic.Count, 4, "测试总数是否相等");
            Assert.AreEqual(dic["test0_3"], "testvalue");
            Assert.AreEqual(dic["test1_3"], "中国");
            Assert.AreEqual(dic["test2_3"], "434324,,,，【】。。。。");
            Assert.AreEqual(dic["测试3_3"], "哈哈");

            dic = client.StringMGet(new string[] { "test0_3", "test1_3", "test2_3", "测试4" });
            Assert.AreEqual(dic.Count, 3, "实际只有3个key是存在的，不存在则会跳过，所以结果是3");
            Assert.AreEqual(dic["test0_3"], "testvalue");
            Assert.AreEqual(dic["test1_3"], "中国");
            Assert.AreEqual(dic["test2_3"], "434324,,,，【】。。。。");
            Assert.AreEqual(false, dic.ContainsKey("测试4"));

            //清理数据
            client.StringDel(new string[] { "test0_3", "test1_3", "test2_3", "测试3_3" });
        }
        /// <summary>
        /// 批量设置指定key的值
        /// </summary>
        [TestMethod]
        public void TestStringMSet()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "A_36","432432"},
                { "B_36","中华"},
                { "大","》》》"},
            };
            Assert.AreEqual(true, client.StringMSet(data));
            data = client.StringMGet(new string[] { "A_36", "B_36", "大" });
            Assert.AreEqual(data["A_36"], "432432");
            Assert.AreEqual(data["B_36"], "中华");
            Assert.AreEqual(data["大"], "》》》");
            bool isOk = client.StringDel(new string[] { "A_36", "B_36", "大" });
            Assert.AreEqual(true, isOk);
        }
        /// <summary>
        /// 设置键值对
        /// </summary>
        [TestMethod]
        public void TestStringSet()
        {
            client.StringSet("A_37", "rewrewr");
            client.StringSet("我", "温热幅度萨芬技术的,fdsf<>《》");
            Dictionary<string, string> dic = client.StringMGet(new string[] { "A_37", "我" });
            Assert.AreEqual(2, dic.Count);
            Assert.AreEqual("rewrewr", dic["A_37"]);

            Assert.AreEqual("温热幅度萨芬技术的,fdsf<>《》", dic["我"]);
            bool isOk = client.StringDel(new string[] { "A_37", "我" });
            Assert.AreEqual(true, isOk);
        }
        /// <summary>
        /// 设置键值对,带过期时间
        /// </summary>
        [TestMethod]
        public void TestStringSetExpire()
        {
            client.StringSet("A_38", "rewrewr",5);
            client.StringSet("我", "温热幅度萨芬技术的,fdsf<>《》", 5);
            Dictionary<string, string> dic = client.StringMGet(new string[] { "A_38", "我" });
            Assert.AreEqual(2, dic.Count);
            Assert.AreEqual("rewrewr", dic["A_38"]);
            Assert.AreEqual("温热幅度萨芬技术的,fdsf<>《》", dic["我"]);
            //休眠6秒后，刚刚添加的值应该已经不存在了
            System.Threading.Thread.Sleep(6000);
            Assert.AreEqual(string.Empty, client.StringGet("A_38"));
            Assert.AreEqual(string.Empty, client.StringGet("我"));
            bool isOk = client.StringDel(new string[] { "A_38", "我" });
            Assert.AreEqual(false, isOk);
        }
        /// <summary>
        /// 设置键值对，注意仅当key值不存在的时候才能设置成功，如果已存在则设置失败
        /// </summary>
        [TestMethod]
        public void TestStringSetNotExist()
        {
            client.StringSet("A_39", "rewrewr");
            bool isOK = client.StringSetNotExist("A_39", "fdsfdsfaf");
            Assert.AreEqual(false, isOK);
            isOK = client.StringSetNotExist("A1", "fdsfdsfaf");
            Assert.AreEqual(true, isOK);
            isOK = client.StringDel(new string[] { "A_39", "A1" });
            Assert.AreEqual(true, isOK);
        }
        /// <summary>
        /// 设置键值对，注意仅当key值不存在的时候才能设置成功，如果已存在则设置失败
        /// </summary>
        [TestMethod]
        public void TestStringSetNotExistExpire()
        {
            client.StringSet("A_40", "rewrewr");
            bool isOK = client.StringSetNotExist("A_40", "fdsfdsfaf", 5);
            Assert.AreEqual(false, isOK);
            isOK = client.StringSetNotExist("A1", "fdsfdsfaf", 5);
            Assert.AreEqual(true, isOK);
            System.Threading.Thread.Sleep(6000);
            Assert.AreEqual(string.Empty, client.StringGet("A1"));
            isOK = client.StringDel("A1");
            Assert.AreEqual(false, isOK);
            isOK = client.StringDel("A_40");
            Assert.AreEqual(true, isOK);
        }
        /// <summary>
        /// 设置键值对，注意仅当key值存在的时候才能设置成功，如果不存在则设置失败
        /// </summary>
        [TestMethod]
        public void TestStringSetExist()
        {
            client.StringSet("A_41", "rewrewr");
            bool isOK = client.StringSetExist("A_41", "fdsfdsfaf");
            Assert.AreEqual(true, isOK);
            isOK = client.StringSetExist("A1_1", "fdsfdsfaf");
            Assert.AreEqual(false, isOK);
            isOK = client.StringDel("A_41");
            Assert.AreEqual(true, isOK);
            isOK = client.StringDel("A1_1");
            Assert.AreEqual(false, isOK);
        }
        /// <summary>
        /// 设置键值对，注意仅当key值存在的时候才能设置成功，如果不存在则设置失败
        /// </summary>
        [TestMethod]
        public void TestStringSetExistExpire()
        {
            client.StringSet("A_42", "rewrewr");
            bool isOK = client.StringSetExist("A_42", "fdsfdsfaf",5);
            Assert.AreEqual(true, isOK);
            isOK = client.StringSetExist("A1_2", "fdsfdsfaf",5);
            Assert.AreEqual(false, isOK);
            System.Threading.Thread.Sleep(6000);
            isOK = client.StringDel("A_41");
            Assert.AreEqual(false, isOK);
            isOK = client.StringDel("A1_2");
            Assert.AreEqual(false, isOK);
        }
        /// <summary>
        /// 设置新值并且返回旧值，如果键不存在，则返回空
        /// </summary>
        [TestMethod]
        public void TestStringGetSet()
        {
            client.StringSet("A_43", "rewrewr");
            string value = client.StringGetSet("A_43", "newvalue");
            Assert.AreEqual("rewrewr", value);
            value = client.StringGetSet("B_43", "newvalue");
            Assert.AreEqual(string.Empty, value);
            Assert.AreEqual("newvalue", client.StringGet("B_43"));
            bool isOK = client.StringDel("A_43");
            Assert.AreEqual(true, isOK);
            isOK = client.StringDel("B_43");
            Assert.AreEqual(true, isOK);
        }
        /// <summary>
        /// 删除key
        /// </summary>
        [TestMethod]
        public void TestStringDel()
        {
            client.StringSet("A_44", "rewrewr");
            bool isOK = client.StringDel("A_44");
            Assert.AreEqual(true, isOK);
            isOK = client.StringDel("A1_3");
            Assert.AreEqual(false, isOK);
        }
        /// <summary>
        /// 批量删除key
        /// </summary>
        [TestMethod]
        public void TestStringDelBatch()
        {
            client.StringSet("A_45", "rewrewr");
            client.StringSet("A2_1", "rewrewr");
            client.StringSet("A3_1", "rewrewr");
            bool isOK = client.StringDel(new string[] { "A_45","A2_1","A3_1"});
            Assert.AreEqual(true, isOK);
            isOK = client.StringDel(new string[] { "A_45", "A2_1", "A3_1" });
            Assert.AreEqual(false, isOK);
        }
        /// <summary>
        /// 给key设置过期时间，单位为秒
        /// </summary>
        [TestMethod]
        public void TestStringExpire()
        {
            client.StringSet("A_46", "rewrewr");
            bool isOK = client.StringExpire("A_46", 5);
            Assert.AreEqual(true, isOK);
            System.Threading.Thread.Sleep(6000);
            Assert.AreEqual(string.Empty, client.StringGet("A_46"));
            isOK = client.StringDel("A_46");
            Assert.AreEqual(false, isOK);
        }
        /// <summary>
        /// 利用redis来实现一个分布式加锁的功能
        /// </summary>
        [TestMethod]
        public void TestStringLock()
        {
            bool isok = client.StringSet("k1", "liusai");
            Assert.AreEqual(true, isok);
            isok = client.StringLock("k1", "liusai", 3);
            Assert.AreEqual(false, isok, "因为键已存在，所以加锁失败");
            isok = client.StringDel("k1");
            Assert.AreEqual(true, isok);
            isok = client.StringLock("k1", "liusai", 3);
            Assert.AreEqual(true, isok, "因为键不存在，所以加锁成功");
            System.Threading.Thread.Sleep(4000);
            string msg = client.StringGet("k1");
            Assert.AreEqual(string.Empty, msg);
        }
        /// <summary>
        /// 利用redis来实现一个分布式解锁的功能
        /// </summary>
        [TestMethod]
        public void TestStringUnLock()
        {
            bool isok = client.StringSetNotExist("k1_unlock", "yy");
            Assert.AreEqual(true, isok);
            isok = client.StringUnLock("k1_unlock", "yy");
            Assert.AreEqual(true, isok, "key值存在同时请求标识等于yy，所以可以解锁");

            isok = client.StringSetNotExist("k1_unlock", "yy2");
            Assert.AreEqual(true, isok);
            isok = client.StringUnLock("k1_unlock", "yy");
            Assert.AreEqual(false, isok, "key值存在同时请求标识不等于yy，所以解锁失败");

            isok = client.StringDel("k1_unlock");
            Assert.AreEqual(true, isok);
        }
        /// <summary>
        /// 执行Lua脚本，注意该方法的返回值是没有经过过滤的，是从redis返回的原始值
        /// </summary>
        [TestMethod]
        public void StringEvalLuaScript()
        {
            string script = "if redis.call('setnx', KEYS[1],ARGV[1]) == 1 then return redis.call('expire', KEYS[1],ARGV[2]) else return 0 end";
            string result = client.StringEvalLuaScript(script, new String[] { "key_99765" }, new String[] { "testv", "10" });
            Assert.AreEqual(":1\r\n", result);

            script = "if redis.call('get', KEYS[1]) == ARGV[1] then return redis.call('del', KEYS[1]) else return 0 end";
            result = client.StringEvalLuaScript(script, new String[] { "key_99765" }, new String[] { "testv" });
            Assert.AreEqual(":1\r\n", result);
        }
    }
}
