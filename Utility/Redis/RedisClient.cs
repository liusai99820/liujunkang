using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Sockets;

namespace Utility.Redis
{
    public partial class RedisClient : IDisposable
    {
        private readonly Socket socket;
        private readonly int database;
        private bool opened;
        public RedisClient(int databaseNum)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            database = databaseNum;
        }
        public void Open()
        {
            if (!opened)
            {
                socket.Connect(ConnectionManager.Host, ConnectionManager.Port);
                if (!string.IsNullOrWhiteSpace(ConnectionManager.Password))
                {
                    socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$4\r\nauth\r\n${0}\r\n{1}\r\n", ConnectionManager.Password.Length, ConnectionManager.Password)));
                    string content = RedisHelper.Receive(socket);
                    if (!content.StartsWith("+OK"))
                    {
                        throw new RedisException("密码验证失败，无法访问Redis");
                    }
                }
                socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$6\r\nselect\r\n${0}\r\n{1}\r\n", database.ToString().Length, database)));
                string returnValue = RedisHelper.Receive(socket);
                if (returnValue.StartsWith("-NOAUTH Authentication required"))
                {
                    throw new RedisException("访问redis需要密码，请在配置文件中设置密码信息");
                }
                if (!returnValue.StartsWith("+OK"))
                {
                    throw new RedisException("数据库索引超出范围");
                }
                opened = true;
            }
        }
        public void Dispose()
        {
            if (socket.Connected)
                socket.Shutdown(SocketShutdown.Both);
        }
    }
}
