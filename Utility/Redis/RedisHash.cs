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
        /// 设置哈希值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段名</param>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public bool HashSet(string key, string field, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*4\r\n$4\r\nhset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(field), field, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith(":1") || result.StartsWith(":0");
        }
        /// <summary>
        /// 设置哈希值，注意只有在字段不存在的时候才能设置哈希值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段名</param>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public bool HashSetNotExist(string key, string field, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*4\r\n$6\r\nhsetnx\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(field), field, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith(":1");
        }
        /// <summary>
        /// 批量设置Hash值
        /// </summary>
        /// <param name="data">字段</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool HashMSet(string key, Dictionary<string, string> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$5\r\nhmset\r\n${1}\r\n{2}\r\n", data.Count * 2 + 2, RedisHelper.GetLength(key), key);
            foreach (var item in data)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(item.Key), item.Key, RedisHelper.GetLength(item.Value), item.Value);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith("+OK");
        }
        /// <summary>
        /// 获取指定key和field的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public string HashGet(string key, string field)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$4\r\nhget\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(field), field)));
            string result = RedisHelper.Receive(socket);
            if (result.StartsWith("$-1"))
                return string.Empty;
            return RedisHelper.ClearString(result);
        }
        /// <summary>
        /// 获取指定key下的所有字段
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public List<string> HashGetFields(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$5\r\nhkeys\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            if (result.StartsWith("*0"))
                return new List<string>();
            return RedisHelper.ClearString2(result);
        }
        /// <summary>
        /// 批量获取指定字段的值,如果查不到值则略过
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">指定字段</param>
        /// <returns></returns>
        public Dictionary<string, string> HashMGet(string key, string[] field)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$5\r\nhmget\r\n${1}\r\n{2}\r\n", field.Length + 2, RedisHelper.GetLength(key), key);
            for (int i = 0, len = field.Length; i < len; i++)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(field[i]), field[i]);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            List<string> values = RedisHelper.ClearString3(result);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            for (int i = 0, len = field.Length; i < len; i++)
            {
                if (!dic.ContainsKey(values[i]))
                {
                    if (!string.Equals("~!@#$%^&*()Wr0KjVC", values[i]))
                        dic.Add(field[i], values[i]);
                }
            }
            return dic;
        }
        /// <summary>
        /// 获取指定key下所有的字段以及对应的值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Dictionary<string, string> HashMGetAll(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$7\r\nhgetall\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            if (result.StartsWith("*0"))
                return new Dictionary<string, string>();
            return RedisHelper.ClearString4(result);
        }
        /// <summary>
        /// 查询key中指定的字段是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public bool HashExistField(string key, string field)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$7\r\nhexists\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(field), field)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith(":1");
        }
        /// <summary>
        /// 删除一个或多个字段
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">一个或多个字段</param>
        /// <returns>删除成功的个数</returns>
        public int HashDelField(string key, string[] field)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$4\r\nhdel\r\n${1}\r\n{2}\r\n", field.Length + 2, RedisHelper.GetLength(key), key);
            for (int i = 0, len = field.Length; i < len; i++)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(field[i]), field[i]);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            return RedisHelper.ClearString5(result);
        }
    }
}
