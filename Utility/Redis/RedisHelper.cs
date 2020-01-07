using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utility.Redis
{
    /// <summary>
    /// 助手
    /// </summary>
    public static class RedisHelper
    {
        //每次从流中读取的最大字节数
        private const int BYTELENGTH = 1024;
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static string Receive(Socket socket)
        {
            string result = string.Empty;
            byte[] buffer = new byte[BYTELENGTH];
            List<byte> bytes = new List<byte>();
            int length = 0;
            int sign = 0;
            do
            {
                length = socket.Receive(buffer, buffer.Length, SocketFlags.None);
                bytes.AddRange(buffer.Take(length));
                string temp = Encoding.UTF8.GetString(buffer.Take(length).ToArray());
                if (sign == 0)
                {
                    string temp1 = Regex.Replace(temp, @"\$\d+\r\n", "#");
                    if (temp1.Length == 1)
                    {
                        length = BYTELENGTH;
                    }
                }
                if (temp.ToLower().StartsWith("*3\r\n$9\r\nsubscribe\r\n") && length != BYTELENGTH)
                {
                    length = BYTELENGTH;
                }
                sign++;
            } while (length == BYTELENGTH);
            return Encoding.UTF8.GetString(bytes.ToArray());
        }
        public static string ClearString(string originValue)
        {
            int startIndex = originValue.IndexOf("\r\n");
            int endIndex = originValue.LastIndexOf("\r\n");
            if (startIndex == endIndex)
                return originValue;
            return originValue.Substring(startIndex + 2, endIndex - startIndex - 2);
        }
        public static List<string> ClearString2(string originValue)
        {
            originValue = ClearString(originValue);
            var list = originValue.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return list.Select((a, i) => new { Value = a, Index = i }).Where(b => b.Index % 2 != 0).Select(c => c.Value).ToList();
        }
        public static Dictionary<string, string> ClearString6(string originValue)
        {
            int startIndex = originValue.IndexOf("*", 1);
            originValue = originValue.Substring(startIndex);
            if (originValue.StartsWith("*0"))
                return new Dictionary<string, string>();
            originValue = ClearString(originValue);
            var list = originValue.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            list = list.Select((a, i) => new { Value = a, Index = i }).Where(b => b.Index % 2 != 0).Select(c => c.Value).ToList();
            Dictionary<string, string> result = new Dictionary<string, string>();
            for (int i = 0, len = list.Count; i < len; i = i + 2)
            {
                result.Add(list[i], list[i + 1]);
            }
            return result;
        }
        public static List<string> ClearString7(string originValue)
        {
            int startIndex = originValue.IndexOf("*", 1);
            originValue = originValue.Substring(startIndex);
            if (originValue.StartsWith("*0"))
                return new List<string>();
            originValue = ClearString(originValue);
            var list = originValue.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return list.Select((a, i) => new { Value = a, Index = i }).Where(b => b.Index % 2 != 0).Select(c => c.Value).ToList();
        }
        public static List<string> ClearString3(string originValue)
        {
            originValue = Regex.Replace(originValue, @"\$-1\r\n", "$-1\r\n~!@#$%^&*()Wr0KjVC\r\n");
            originValue = ClearString(originValue);
            originValue = Regex.Replace(originValue, @"\r\n", "\r");
            var list = originValue.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return list.Select((a, i) => new { Value = a, Index = i }).Where(b => b.Index % 2 != 0).Select(c => c.Value).ToList();
        }
        public static Dictionary<string,string> ClearString4(string originValue)
        {
            originValue = ClearString(originValue);
            var list = originValue.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            list = list.Select((a, i) => new { Value = a, Index = i }).Where(b => b.Index % 2 != 0).Select(c => c.Value).ToList();
            Dictionary<string, string> result = new Dictionary<string, string>();
            for (int i = 0, len = list.Count; i < len; i = i + 2)
            {
                result.Add(list[i], list[i + 1]);
            }
            return result;
        }
        public static int ClearString5(string originValue)
        {
            int startIndex = originValue.IndexOf(":");
            int endIndex = originValue.LastIndexOf("\r\n");
            int.TryParse(originValue.Substring(startIndex + 1, endIndex - startIndex), out int result);
            return result;
        }
        /// <summary>
        /// 用正则表达式 判断字符是不是汉字或中文标点
        /// </summary>
        /// <param name="text">待判断字符或字符串</param>
        /// <returns>真：是汉字；假：不是</returns>
        public static bool CheckStringChineseReg(string text)
        {
            return Regex.IsMatch(text, @"[\u4e00-\u9fbb]|[，。！、【】（）？；：《》￥……]");
        }
        public static int GetLength(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;
            int sign = 0;
            for (int i = 0, len = text.Length; i < len; i++)
            {
                if (CheckStringChineseReg(text[i].ToString()))
                {
                    sign = sign + 3;
                }
                else
                {
                    sign = sign + 1;
                }
            }
            return sign;
        }
        /// <summary>
        /// 判断服务器是否可以正常连接
        /// </summary>
        /// <param name="host">主机地址</param>
        /// <param name="port">端口号</param>
        /// <param name="millisecondTimeout">超时时间（单位毫秒）</param>
        /// <returns></returns>
        public static bool IsConnected(string host, int port, int millisecondTimeout)
        {
            using (TcpClient client = new TcpClient())
            {
                var ar = client.BeginConnect(host, port, null, null);
                ar.AsyncWaitHandle.WaitOne(millisecondTimeout);
                return client.Connected;
            }
        }
    }
}
