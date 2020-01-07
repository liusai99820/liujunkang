using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Redis
{
    public partial class ClusterClient
    {
        private readonly string cluster_host;
        private readonly int cluster_port;
        private string return_value;
        public ClusterClient()
        {
            string cluster = ClusterManager.Cluster;
            if (string.IsNullOrWhiteSpace(cluster))
                throw new RedisException("请配置集群的相关参数");
            string[] arr = cluster.Split(',');
            for (int i = 0, len = arr.Length; i < len; i++)
            {
                string[] tempArr = arr[i].Split(':');
                string host = tempArr[0];
                int.TryParse(tempArr[1], out int port);
                if (RedisHelper.IsConnected(host, port, 1000))
                {
                    cluster_host = host;
                    cluster_port = port;
                    break;
                }
            }
        }
        public RedisClient GetInstance(byte[] command)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Connect(cluster_host, cluster_port);
            if (!string.IsNullOrWhiteSpace(ClusterManager.Password))
            {
                socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$4\r\nauth\r\n${0}\r\n{1}\r\n", ClusterManager.Password.Length, ClusterManager.Password)));
                string content = RedisHelper.Receive(socket);
                if (!content.StartsWith("+OK"))
                {
                    throw new RedisException("密码验证失败，无法访问Redis");
                }
            }
            socket.Send(command);
            string returnValue = RedisHelper.Receive(socket);
            return_value = returnValue;
            if (returnValue.StartsWith("-MOVED"))
            {
                returnValue = returnValue.Split(' ')[2].TrimEnd(new char[] { '\r', '\n' });
                string[] temp = returnValue.Split(':');
                string master_host = temp[0];
                int.TryParse(temp[1], out int master_port);
                socket.Shutdown(SocketShutdown.Both);
                RedisClient client = new RedisClient(-1, master_host, master_port, ClusterManager.Password);
                client.Open();
                return client;
            }
            return null;
        }
    }
}
