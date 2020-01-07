using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Redis
{
    public static class ClusterManager
    {
        private static readonly string cluster;
        private static readonly string password;
        static ClusterManager()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["RedisCluster"].ToString();
            string[] arr = connectionString.Split(';');
            Dictionary<string, string> tempDic = new Dictionary<string, string>();
            for (int i = 0, len = arr.Length; i < len; i++)
            {
                var tempItem = arr[i].Split('=');
                if (!tempDic.ContainsKey(tempItem[0]))
                    tempDic.Add(tempItem[0], tempItem[1]);
            }
            tempDic.TryGetValue("cluster", out string _cluster);
            tempDic.TryGetValue("password", out string _password);
            password = _password;
            cluster = _cluster;
        }
        /// <summary>
        /// 连接Redis的密码，对应redis.conf中的requirepass
        /// </summary>
        public static string Password
        {
            get
            {
                return password;
            }
        }
        /// <summary>
        /// redis集群服务器地址及端口号，多个节点之间用逗号隔开
        /// </summary>
        public static string Cluster
        {
            get
            {
                return cluster;
            }
        }
    }
}
