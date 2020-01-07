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
        /// 获取Key对应的Value
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*2\r\n$3\r\nget\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                if (this.return_value.StartsWith("$-1"))
                    return string.Empty;
                return RedisHelper.ClearString(this.return_value);
            }
            using (client)
            {
                return client.StringGet(key);
            }
        }
        /// <summary>
        /// 判断指定的key是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool StringExists(string key)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*2\r\n$6\r\nexists\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith(":1");
            }
            using (client)
            {
                return client.StringExists(key);
            }
        }
        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool StringSet(string key, string value)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*3\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith("+OK");
            }
            using (client)
            {
                return client.StringSet(key, value);
            }
        }
        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间，单位为秒</param>
        /// <returns></returns>
        public bool StringSet(string key, string value, int expire)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*5\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n$2\r\nEX\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value, expire.ToString().Length, expire));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith("+OK");
            }
            using (client)
            {
                return client.StringSet(key, value, expire);
            }
        }
        /// <summary>
        /// 设置键值对，注意仅当key值不存在的时候才能设置成功，如果已存在则设置失败
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool StringSetNotExist(string key, string value)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*4\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n$2\r\nNX\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith("+OK");
            }
            using (client)
            {
                return client.StringSetNotExist(key, value);
            }
        }
        /// <summary>
        /// 设置键值对，注意仅当key值不存在的时候才能设置成功，如果已存在则设置失败
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间，单位为秒</param>
        /// <returns></returns>
        public bool StringSetNotExist(string key, string value, int expire)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*6\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n$2\r\nEX\r\n${4}\r\n{5}\r\n$2\r\nNX\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value, expire.ToString().Length, expire));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith("+OK");
            }
            using (client)
            {
                return client.StringSetNotExist(key, value, expire);
            }
        }
        /// <summary>
        /// 设置键值对，注意仅当key值存在的时候才能设置成功，如果不存在则设置失败
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool StringSetExist(string key, string value)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*4\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n$2\r\nXX\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith("+OK");
            }
            using (client)
            {
                return client.StringSetExist(key, value);
            }
        }
        /// <summary>
        /// 设置键值对，注意仅当key值存在的时候才能设置成功，如果不存在则设置失败
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间，单位为秒</param>
        /// <returns></returns>
        public bool StringSetExist(string key, string value, int expire)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*6\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n$2\r\nEX\r\n${4}\r\n{5}\r\n$2\r\nXX\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value, expire.ToString().Length, expire));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith("+OK");
            }
            using (client)
            {
                return client.StringSetExist(key, value, expire);
            }
        }
        /// <summary>
        /// 设置新值并且返回旧值，如果键不存在，则返回空
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>旧值</returns>
        public string StringGetSet(string key, string value)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*3\r\n$6\r\ngetset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                if (this.return_value.StartsWith("$-1"))
                    return string.Empty;
                return RedisHelper.ClearString(this.return_value);
            }
            using (client)
            {
                return client.StringGetSet(key, value);
            }
        }
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool StringDel(string key)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*2\r\n$3\r\ndel\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return RedisHelper.ClearString5(this.return_value) > 0; ;
            }
            using (client)
            {
                return client.StringDel(key);
            }
        }
        /// <summary>
        /// 给key设置过期时间，单位为秒
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="seconds">过期时间</param>
        /// <returns></returns>
        public bool StringExpire(string key, int seconds)
        {
            byte[] command = Encoding.UTF8.GetBytes(string.Format("*3\r\n$6\r\nexpire\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, seconds.ToString().Length, seconds));
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return this.return_value.StartsWith(":1");
            }
            using (client)
            {
                return client.StringExpire(key, seconds);
            }
        }
        /// <summary>
        /// 执行Lua脚本，注意该方法的返回值是没有经过过滤的，是从redis返回的原始值
        /// </summary>
        /// <param name="script">Lua脚本 For example:"if redis.call('get', KEYS[1]) == ARGV[1] then return redis.call('del', KEYS[1]) else return 0 end"</param>
        /// <param name="keys">KEYS数组</param>
        /// <param name="argv">参数数组</param>
        /// <returns></returns>
        public string StringEvalLuaScript(string script, string[] keys, string[] argv)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$4\r\neval\r\n${1}\r\n{2}\r\n${3}\r\n{4}\r\n", 3 + keys.Length + argv.Length, script.Length, script, keys.Length.ToString().Length, keys.Length);
            foreach (var item in keys)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            foreach (var item in argv)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            byte[] command = Encoding.UTF8.GetBytes(sb.ToString());
            RedisClient client = this.GetInstance(command);
            if (client == null)
            {
                return RedisHelper.ClearString(this.return_value);
            }
            using (client)
            {
                return client.StringEvalLuaScript(script, keys, argv);
            }
        }
        /// <summary>
        /// 利用redis来实现一个分布式加锁的功能
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="requestId">请求标识</param>
        /// <param name="expire">锁的过期时间，单位为秒</param>
        /// <returns></returns>
        public bool StringLock(string key, string requestId, int expire)
        {
            string script = "if redis.call('setnx', KEYS[1],ARGV[1]) == 1 then return redis.call('expire', KEYS[1],ARGV[2]) else return 0 end";
            string result = StringEvalLuaScript(script, new string[] { key }, new string[] { requestId, expire.ToString() });
            return RedisHelper.ClearString5(result) > 0;
        }

        /// <summary>
        /// 利用redis来实现一个分布式解锁的功能
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="requestId">请求标识</param>
        /// <returns></returns>
        public bool StringUnLock(string key, string requestId)
        {
            string script = "if redis.call('get', KEYS[1]) == ARGV[1] then return redis.call('del', KEYS[1]) else return 0 end";
            string result = StringEvalLuaScript(script, new string[] { key }, new string[] { requestId });
            return RedisHelper.ClearString5(result) > 0;
        }
    }
}
