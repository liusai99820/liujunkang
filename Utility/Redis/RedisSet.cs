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
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 获取集合的成员个数
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public int SetCard(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$5\r\nscard\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 返回多个集合的差集
        /// </summary>
        /// <param name="keys">多个key</param>
        /// <returns></returns>
        public List<string> SetDiff(string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$5\r\nsdiff\r\n", keys.Length + 1);
            foreach (var item in keys)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return new List<string>();
            //key值不存在
            if (result.StartsWith("*0"))
                return new List<string>();
            return RedisHelper.ClearString2(result);
        }
        /// <summary>
        /// 返回多个集合的差集并存储在destination中
        /// </summary>
        /// <param name="destination">目标key</param>
        /// <param name="keys">多个key</param>
        /// <returns></returns>
        public int SetDiffStore(string destination, string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$10\r\nsdiffstore\r\n${1}\r\n{2}\r\n", keys.Length + 2, RedisHelper.GetLength(destination), destination);
            foreach (var item in keys)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 返回多个集合间的交集
        /// </summary>
        /// <param name="keys">多个key</param>
        /// <returns></returns>
        public List<string> SetInter(string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$6\r\nsinter\r\n", keys.Length + 1);
            foreach (var item in keys)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return new List<string>();
            //key值不存在
            if (result.StartsWith("*0"))
                return new List<string>();
            return RedisHelper.ClearString2(result);
        }
        /// <summary>
        /// 返回多个集合间的交集并存储在destination中
        /// </summary>
        /// <param name="destination">目标key</param>
        /// <param name="keys">多个key</param>
        /// <returns></returns>
        public int SetInterStore(string destination, string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$11\r\nsinterstore\r\n${1}\r\n{2}\r\n", keys.Length + 2, RedisHelper.GetLength(destination), destination);
            foreach (var item in keys)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 判断成员元素是否是集合的成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetSisMember(string key, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$9\r\nsismember\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            return result.StartsWith(":1");
        }
        /// <summary>
        /// 返回集合key中所有的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public List<string> SetMembers(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$8\r\nsmembers\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return new List<string>();
            //key值不存在
            if (result.StartsWith("*0"))
                return new List<string>();
            return RedisHelper.ClearString2(result);
        }
        /// <summary>
        /// 将元素value从from移动到to
        /// </summary>
        /// <param name="from">源集合</param>
        /// <param name="to">目标集合</param>
        /// <param name="value">元素</param>
        /// <returns></returns>
        public int SetMove(string from, string to, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*4\r\n$5\r\nsmove\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(from), from, RedisHelper.GetLength(to), to, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 随机移除指定数量的元素并返回
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">个数</param>
        /// <returns></returns>
        public List<string> SetPop(string key, int count)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$4\r\nspop\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(count.ToString()), count)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return new List<string>();
            //key值不存在
            if (result.StartsWith("*0"))
                return new List<string>();
            return RedisHelper.ClearString2(result);
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
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 返回多个集合的并集
        /// </summary>
        /// <param name="keys">多个key</param>
        /// <returns></returns>
        public List<string> SetUnion(string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$6\r\nsunion\r\n", keys.Length + 1);
            foreach (var item in keys)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return new List<string>();
            //key值不存在
            if (result.StartsWith("*0"))
                return new List<string>();
            return RedisHelper.ClearString2(result);
        }
        /// <summary>
        /// 返回多个集合的并集并存储在destination中
        /// </summary>
        /// <param name="destination">目标key</param>
        /// <param name="keys">多个key</param>
        /// <returns></returns>
        public int SetUnionStore(string destination, string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$11\r\nsunionstore\r\n${1}\r\n{2}\r\n", keys.Length + 2, RedisHelper.GetLength(destination), destination);
            foreach (var item in keys)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 根据模式检索满足条件的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="match">例如*h，查询以h结尾的元素</param>
        /// <returns></returns>
        public List<string> SetScan(string key, string match)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*5\r\n$5\r\nsscan\r\n${0}\r\n{1}\r\n$1\r\n0\r\n$5\r\nmatch\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(match), match)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return new List<string>();
            //key值不存在
            if (result.StartsWith("*0"))
                return new List<string>();
            return RedisHelper.ClearString7(result);
        }
    }
}
