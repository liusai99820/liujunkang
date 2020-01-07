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
        /// 向集合中添加一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">一个或多个成员</param>
        /// <returns></returns>
        public int SetAdd(string key, string[] values)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$4\r\nsadd\r\n${1}\r\n{2}\r\n", values.Length + 2, RedisHelper.GetLength(key), key);
            foreach (var item in values)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            byte[] command = Encoding.UTF8.GetBytes(sb.ToString());
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                //key值已经表示其它类型了，所以无法执行命令
                if (this.return_value.StartsWith("-WRONGTYPE"))
                    return 0;
                return RedisHelper.ClearString5(this.return_value);
            }
            using (client)
            {
                return client.SetAdd(key, values);
            }
        }
        /// <summary>
        /// 获取集合的成员个数
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public int SetCard(string key)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*2\r\n$5\r\nscard\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return RedisHelper.ClearString5(this.return_value);
            }
            using (client)
            {
                return client.SetCard(key);
            }
        }
        /// <summary>
        /// 判断成员元素是否是集合的成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetSisMember(string key, string value)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*3\r\n$9\r\nsismember\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith(":1");
            }
            using (client)
            {
                return client.SetSisMember(key, value);
            }
        }
        /// <summary>
        /// 返回集合key中所有的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public List<string> SetMembers(string key)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*2\r\n$8\r\nsmembers\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                //key值已经表示其它类型了，所以无法执行命令
                if (this.return_value.StartsWith("-WRONGTYPE"))
                    return new List<string>();
                //key值不存在
                if (this.return_value.StartsWith("*0"))
                    return new List<string>();
                return RedisHelper.ClearString2(this.return_value);
            }
            using (client)
            {
                return client.SetMembers(key);
            }
        }
        /// <summary>
        /// 随机移除指定数量的元素并返回
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">个数</param>
        /// <returns></returns>
        public List<string> SetPop(string key, int count)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*3\r\n$4\r\nspop\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(count.ToString()), count));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                //key值已经表示其它类型了，所以无法执行命令
                if (this.return_value.StartsWith("-WRONGTYPE"))
                    return new List<string>();
                //key值不存在
                if (this.return_value.StartsWith("*0"))
                    return new List<string>();
                return RedisHelper.ClearString2(this.return_value);
            }
            using (client)
            {
                return client.SetPop(key, count);
            }
        }
        /// <summary>
        /// 移除集合中的一个或多个元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">一个或多个值</param>
        /// <returns></returns>
        public int SetRem(string key, string[] values)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$4\r\nsrem\r\n${1}\r\n{2}\r\n", values.Length + 2, RedisHelper.GetLength(key), key);
            foreach (var item in values)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            byte[] command = Encoding.UTF8.GetBytes(sb.ToString());
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                //key值已经表示其它类型了，所以无法执行命令
                if (this.return_value.StartsWith("-WRONGTYPE"))
                    return 0;
                return RedisHelper.ClearString5(this.return_value);
            }
            using (client)
            {
                return client.SetRem(key, values);
            }
        }
        /// <summary>
        /// 根据模式检索满足条件的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="match">例如*h，查询以h结尾的元素</param>
        /// <returns></returns>
        public List<string> SetScan(string key, string match)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*5\r\n$5\r\nsscan\r\n${0}\r\n{1}\r\n$1\r\n0\r\n$5\r\nmatch\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(match), match));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                //key值已经表示其它类型了，所以无法执行命令
                if (this.return_value.StartsWith("-WRONGTYPE"))
                    return new List<string>();
                //key值不存在
                if (this.return_value.StartsWith("*0"))
                    return new List<string>();
                return RedisHelper.ClearString7(this.return_value);
            }
            using (client)
            {
                return client.SetScan(key, match);
            }
        }
    }
}
