using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Redis
{
    /// <summary>
    /// 解析Redis数据库连接串
    /// </summary>
    public static class CommonManager
    {
        private static readonly string host;
        private static readonly int port;
        private static readonly string password;
        static CommonManager()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["RedisExchangeHosts"].ToString();
            string[] arr = connectionString.Split(';');
            Dictionary<string, string> tempDic = new Dictionary<string, string>();
            for (int i = 0, len = arr.Length; i < len; i++)
            {
                var tempItem = arr[i].Split('=');
                if (!tempDic.ContainsKey(tempItem[0]))
                    tempDic.Add(tempItem[0], tempItem[1]);
            }
            tempDic.TryGetValue("host", out string _host);
            tempDic.TryGetValue("port", out string _port);
            tempDic.TryGetValue("password", out string _password);
            int.TryParse(_port, out int intPort);
            host = _host;
            port = intPort;
            password = _password;
        }
        /// <summary>
        /// 主机地址
        /// </summary>
        public static string Host {
            get {
                return host;
            }
        }
        /// <summary>
        /// 端口号
        /// </summary>
        public static int Port
        {
            get {
                return port;
            }
        }
        /// <summary>
        /// 连接Redis的密码，对应redis.conf中的requirepass
        /// </summary>
        public static string Password
        {
            get {
                return password;
            }
        }
    }
}
