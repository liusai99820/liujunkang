using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Redis
{
    public class SentinelClient
    {
        private readonly string sentinel_host;
        private readonly int sentinel_port;
        private readonly int databaseNum;
        public SentinelClient(int _databaseNum)
        {
            string sentinel = SentinelManager.Sentinel;
            if (string.IsNullOrWhiteSpace(sentinel))
                throw new RedisException("请配置访问哨兵服务器的相关参数");
            string[] arr = sentinel.Split(',');
            for (int i = 0, len = arr.Length; i < len; i++)
            {
                string[] tempArr = arr[i].Split(':');
                string host = tempArr[0];
                int.TryParse(tempArr[1], out int port);
                if (RedisHelper.IsConnected(host, port, 1000))
                {
                    sentinel_host = host;
                    sentinel_port = port;
                    break;
                }
            }
            databaseNum = _databaseNum;
        }
        public RedisClient GetInstance()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Connect(sentinel_host, sentinel_port);
            string command = string.Format("*3\r\n$8\r\nsentinel\r\n$23\r\nget-master-addr-by-name\r\n${0}\r\n{1}\r\n", SentinelManager.Mastername.Length, SentinelManager.Mastername);
            socket.Send(Encoding.UTF8.GetBytes(command));
            string returnValue = RedisHelper.Receive(socket);
            List<string> master_addr = RedisHelper.ClearString2(returnValue);
            if (master_addr.Count == 0)
                throw new RedisException("Redis主服务器不在线");
            string master_host = master_addr[0];
            int.TryParse(master_addr[1], out int master_port);
            socket.Shutdown(SocketShutdown.Both);
            return new RedisClient(databaseNum, master_host, master_port, SentinelManager.Password);
        }
    }
}
