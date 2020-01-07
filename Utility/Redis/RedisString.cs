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
        /// 获取Key对应的Value
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$3\r\nget\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            if (result.StartsWith("$-1"))
                return string.Empty;
            return RedisHelper.ClearString(result);
        }
        /// <summary>
        /// 获取keys集合
        /// </summary>
        /// <param name="value">查询参数</param>
        /// <returns></returns>
        public List<string> StringKeys(string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$4\r\nkeys\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            if (result.StartsWith("*0"))
                return new List<string>();
            return RedisHelper.ClearString2(result);
        }
        /// <summary>
        /// 判断指定的key是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool StringExists(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$6\r\nexists\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith(":1");
        }
        /// <summary>
        /// 批量获取指定key的值,如果查不到值则略过
        /// </summary>
        /// <param name="keys">key数组</param>
        /// <returns></returns>
        public Dictionary<string, string> StringMGet(string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$4\r\nmget\r\n", keys.Length + 1);
            for (int i = 0, len = keys.Length; i < len; i++)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(keys[i]), keys[i]);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            List<string> values = RedisHelper.ClearString3(result);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            for (int i = 0, len = keys.Length; i < len; i++)
            {
                if (!dic.ContainsKey(values[i]))
                {
                    if (!string.Equals("~!@#$%^&*()Wr0KjVC", values[i]))
                        dic.Add(keys[i], values[i]);
                }
            }
            return dic;
        }
        /// <summary>
        /// 批量设置指定key的值
        /// </summary>
        /// <param name="keys">键值对</param>
        /// <returns></returns>
        public bool StringMSet(Dictionary<string, string> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$4\r\nmset\r\n", data.Count * 2 + 1);
            foreach (var item in data)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(item.Key), item.Key, RedisHelper.GetLength(item.Value), item.Value);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith("+OK");
        }
        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool StringSet(string key, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith("+OK");
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
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*5\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n$2\r\nEX\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value, expire.ToString().Length, expire)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith("+OK");
        }
        /// <summary>
        /// 设置键值对，注意仅当key值不存在的时候才能设置成功，如果已存在则设置失败
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool StringSetNotExist(string key, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*4\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n$2\r\nNX\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith("+OK");
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
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*6\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n$2\r\nEX\r\n${4}\r\n{5}\r\n$2\r\nNX\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value, expire.ToString().Length, expire)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith("+OK");
        }
        /// <summary>
        /// 设置键值对，注意仅当key值存在的时候才能设置成功，如果不存在则设置失败
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool StringSetExist(string key, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*4\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n$2\r\nXX\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith("+OK");
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
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*6\r\n$3\r\nset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n$2\r\nEX\r\n${4}\r\n{5}\r\n$2\r\nXX\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value, expire.ToString().Length, expire)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith("+OK");
        }
        /// <summary>
        /// 设置新值并且返回旧值，如果键不存在，则返回空
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>旧值</returns>
        public string StringGetSet(string key, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$6\r\ngetset\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            if (result.StartsWith("$-1"))
                return string.Empty;
            return RedisHelper.ClearString(result);
        }
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool StringDel(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$3\r\ndel\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            return RedisHelper.ClearString5(result) > 0; ;
        }

        /// <summary>
        /// 批量删除key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool StringDel(string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$3\r\ndel\r\n", keys.Length + 1);
            foreach (var item in keys)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            return RedisHelper.ClearString5(result) > 0;
        }

        /// <summary>
        /// 给key设置过期时间，单位为秒
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="seconds">过期时间</param>
        /// <returns></returns>
        public bool StringExpire(string key, int seconds)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$6\r\nexpire\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, seconds.ToString().Length, seconds)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith(":1");
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
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            return RedisHelper.ClearString(result);
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
