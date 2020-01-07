using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Utility.Redis;

namespace ConsoleApp1
{
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
}
