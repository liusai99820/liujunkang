using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Redis
{
    public partial class ClusterClient
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
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*4\r\n$4\r\nhset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(field), field, RedisHelper.GetLength(value), value));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith(":1") || this.return_value.StartsWith(":0");
            }
            using (client)
            {
                return client.HashSet(key, field, value);
            }
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
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*4\r\n$6\r\nhsetnx\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(field), field, RedisHelper.GetLength(value), value));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith(":1");
            }
            using (client)
            {
                return client.HashSetNotExist(key, field, value);
            }
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
            byte[] command = Encoding.UTF8.GetBytes(sb.ToString());
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith("+OK");
            }
            using (client)
            {
                return client.HashMSet(key, data);
            }
        }
        /// <summary>
        /// 获取指定key和field的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public string HashGet(string key, string field)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*3\r\n$4\r\nhget\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(field), field));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                if (this.return_value.StartsWith("$-1"))
                    return string.Empty;
                return RedisHelper.ClearString(this.return_value);
            }
            using (client)
            {
                return client.HashGet(key, field);
            }
        }
        /// <summary>
        /// 获取指定key下的所有字段
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public List<string> HashGetFields(string key)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*2\r\n$5\r\nhkeys\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                if (this.return_value.StartsWith("*0"))
                    return new List<string>();
                return RedisHelper.ClearString2(this.return_value);
            }
            using (client)
            {
                return client.HashGetFields(key);
            }
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
            byte[] command = Encoding.UTF8.GetBytes(sb.ToString());
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                List<string> values = RedisHelper.ClearString3(this.return_value);
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
            using (client)
            {
                return client.HashMGet(key, field);
            }
        }
        /// <summary>
        /// 获取指定key下所有的字段以及对应的值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Dictionary<string, string> HashMGetAll(string key)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*2\r\n$7\r\nhgetall\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                if (this.return_value.StartsWith("*0"))
                    return new Dictionary<string, string>();
                return RedisHelper.ClearString4(this.return_value);
            }
            using (client)
            {
                return client.HashMGetAll(key);
            }
        }
        /// <summary>
        /// 查询key中指定的字段是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public bool HashExistField(string key, string field)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*3\r\n$7\r\nhexists\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(field), field));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith(":1");
            }
            using (client)
            {
                return client.HashExistField(key, field);
            }
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
            byte[] command = Encoding.UTF8.GetBytes(sb.ToString());
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return RedisHelper.ClearString5(this.return_value);
            }
            using (client)
            {
                return client.HashDelField(key, field);
            }
        }
    }
}
