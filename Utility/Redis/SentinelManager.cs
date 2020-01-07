using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Utility.Redis
{
    public static class SentinelManager
    {
        private static readonly string sentinel;
        private static readonly string mastername;
        private static readonly string password;
        static SentinelManager()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["RedisSentinel"].ToString();
            string[] arr = connectionString.Split(';');
            Dictionary<string, string> tempDic = new Dictionary<string, string>();
            for (int i = 0, len = arr.Length; i < len; i++)
            {
                var tempItem = arr[i].Split('=');
                if (!tempDic.ContainsKey(tempItem[0]))
                    tempDic.Add(tempItem[0], tempItem[1]);
            }
            tempDic.TryGetValue("sentinel", out string _sentinel);
            tempDic.TryGetValue("mastername", out string _mastername);
            tempDic.TryGetValue("password", out string _password);
            mastername = _mastername;
            password = _password;
            sentinel = _sentinel;
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
        /// 哨兵要监控的主机名称
        /// sentinel monitor <master-group-name> <ip> <port> <quorum>
        /// </summary>
        public static string Mastername
        {
            get
            {
                return mastername;
            }
        }
        /// <summary>
        /// 哨兵所在服务器地址及端口号，多个哨兵之间用逗号隔开
        /// </summary>
        public static string Sentinel
        {
            get
            {
                return sentinel;
            }
        }
    }
}
