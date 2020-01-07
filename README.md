 ## 目录
* [背景介绍](#背景介绍)
* [项目介绍](#项目介绍)
* [使用说明](#使用说明)
* [联系方式](#联系方式)

 
<a name="背景介绍"></a>
## 背景介绍
 
希望得到一个体积小、功能丰富、使用简单的redis库
 
<a name="项目介绍"></a>
## 项目介绍
 
本项目采用C#开发，基于.netframework4.6.1。共包括66个方法，分别基于redis提供的常用命令实现，每个方法都经过严格测试，能够满足绝大多数访问redis的需求。核心类只有9个。  
ConnectionManager.cs，读取配置文件  
RedisClient.cs，门面，主要用来和客户交互  
RedisHash.cs，对应redis里面的哈希表  
RedisList.cs，对应redis里面的列表  
RedisPublishSubscribe.cs，对应redis里面的发布订阅  
RedisSet.cs，对应redis里面的集合  
RedisSortedSet.cs，对应redis里面的有序集合  
RedisString.cs，对应redis里面的字符串操作  
RedisHelper.cs，辅助类  
 
<a name="使用说明"></a>
## 使用说明
* 1、App.config
```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <!-- 单机模式 -->
    <add name="RedisExchangeHosts" connectionString="host=127.0.0.1;port=8900;password=123456" />
    <!-- 哨兵模式 -->
    <add name="RedisSentinel" connectionString="sentinel=127.0.0.1:8905,127.0.0.1:8906,127.0.0.1:8907;mastername=mymaster;password=123456"/>
    <!-- 集群模式 -->
    <add name="RedisCluster" connectionString="cluster=127.0.0.1:8908,127.0.0.1:8909,127.0.0.1:8910,127.0.0.1:8911,127.0.0.1:8912,127.0.0.1:8913;password=123456"/>
  </connectionStrings>
</configuration>
```
* 2、Console  
```
    class Program  
    {  
        static void Main(string[] args)  
        {  
           #region 单机模式
            Console.WriteLine("******************** Common Start ******************");
            using (RedisClient client = new RedisClient(15))
            {
                client.Open();

                bool result = client.StringSet("A1", "From the roof you were able to see the full extent of the park");
                Console.WriteLine("设置键值对：" + result);
                string msg = client.StringGet("A1");
                Console.WriteLine("获取A1的键值：" + msg);

                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    { "A2","朱门酒肉臭"},
                    { "A3","路有冻死骨"}
                };
                result = client.StringMSet(data);
                Console.WriteLine("批量设置键值对：" + result);
                var dic = client.StringMGet(new string[] { "A2", "A3" });
                Console.WriteLine("批量获取键值对：" + string.Join(",", dic.Select(p => p.Key + "=" + p.Value)));

                result = client.HashSet("A4", "name", "Lily");
                Console.WriteLine("设置哈希值：" + result);
                msg = client.HashGet("A4", "name");
                Console.WriteLine("获取哈希值A4->name：" + msg);

                int nums = client.SetAdd("row", new string[] { "我", "你", "他", "她" });
                Console.WriteLine("成功向集合row中添加了" + nums + "个元素");
                var list = client.SetMembers("row");
                Console.WriteLine("集合row中的成员是：" + string.Join(",", list));

                nums = client.SetAdd("row2", new string[] { "我", "你", "坏人", "好人" });
                Console.WriteLine("成功向集合row2中添加了" + nums + "个元素");
                list = client.SetMembers("row2");
                Console.WriteLine("集合row2中的成员是：" + string.Join(",", list));

                list = client.SetUnion(new string[] { "row", "row2" });
                Console.WriteLine("集合row和集合row2的并集是：" + string.Join(",", list));

                list = client.SetInter(new string[] { "row", "row2" });
                Console.WriteLine("集合row和集合row2的交集是：" + string.Join(",", list));

                result = client.StringSet("昙花", "10秒后自动删除", 10);
                Console.WriteLine("设置键值对，带过期时间：" + result);

                nums = client.ListLPush("陕西", new string[] { "西安", "铜川", "宝鸡", "咸阳", "延安" });
                Console.WriteLine("成功向列表头部推入" + nums + "个元素");
                list = client.ListLRange("陕西", 0, 10);
                Console.WriteLine("列表<陕西>中的元素是：" + string.Join(",", list));

                result = client.StringDel(new string[] { "A1", "A2", "A3", "A4", "row", "row2", "陕西" });
                Console.WriteLine("批量删除元素：" + result);
            }
            Console.WriteLine("******************** Common End ******************");
            #endregion 单机模式
            Console.WriteLine();
            #region 哨兵模式
            Console.WriteLine("******************** Sentinel Start ******************");
            SentinelClient sentinel = new SentinelClient(1);
            using (RedisClient client = sentinel.GetInstance())
            {
                client.Open();
                bool result_0 = client.StringSet("name", "helloworld");
                Console.WriteLine("哨兵模式设置：" + (result_0 ? "成功" : "失败"));
                Console.WriteLine(client.StringGet("name"));
            }
            Console.WriteLine("******************** Sentinel End ******************");
            #endregion 哨兵模式
            Console.WriteLine();
            #region 集群模式
            Console.WriteLine("******************** Cluster Start ******************");
            ClusterClient cluster = new ClusterClient();
            string test1 = cluster.StringGet("name");
            Console.WriteLine("name="+test1);
            bool reslut_1 = cluster.StringSet("test1", "寒假就要到了");
            Console.WriteLine("集群设置：" + (reslut_1 ? "成功" : "失败"));
            test1 = cluster.StringGet("test1");
            Console.WriteLine("test1=" + test1);
            Console.WriteLine("******************** Cluster End ******************");
            #endregion 集群模式
        }
    }
 ```   
* 3、注意如果你使用的redis没有设置密码，那么则可以忽略掉  
```
<add name="RedisExchangeHosts" connectionString="host=127.0.0.1;port=6379;" />  
```
 
## 联系方式
欢迎随时与我沟通：  
Please feel free to contact me at any time. The contact information is as follows：  
 

* Email：liusai99820@qq.com
* QQ：598241877
* Wechat：liusai99820
