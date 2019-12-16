using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Redis
{
    public partial class RedisClient
    {
        /// <summary>
        /// 订阅频道
        /// </summary>
        /// <param name="channel">频道名称</param>
        /// <returns></returns>
        public string SubscribeChannel(string channel)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$9\r\nSUBSCRIBE\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(channel), channel)));
            string result = RedisHelper.Receive(socket);
            result = RedisHelper.ClearString(result);
            int start = result.LastIndexOf("\r\n");
            result = result.Substring(start + 2);
            return result;
        }
        /// <summary>
        /// 向指定频道发布消息
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="message">消息</param>
        public void PublishMessage(string channel, string message)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$7\r\nPUBLISH\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(channel), channel, RedisHelper.GetLength(message), message)));
        }
    }
}
