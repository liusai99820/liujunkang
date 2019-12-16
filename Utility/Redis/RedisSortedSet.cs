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
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">分数及对应的成员</param>
        /// <returns></returns>
        public int SortedSetAdd(string key, Dictionary<int, string> values)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$4\r\nzadd\r\n${1}\r\n{2}\r\n", values.Count * 2 + 2, RedisHelper.GetLength(key), key);
            foreach (var item in values)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(item.Key.ToString()), item.Key, RedisHelper.GetLength(item.Value), item.Value);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 返回有序集合中元素的数量
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public int SortedSetZcard(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$5\r\nzcard\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小分数值</param>
        /// <param name="max">最大分数值</param>
        /// <returns></returns>
        public int SortedSetZcount(string key, int min, int max)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*4\r\n$6\r\nzcount\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(min.ToString()), min, RedisHelper.GetLength(max.ToString()), max)));
            string result = RedisHelper.Receive(socket);
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 计算给定的一个或多个有序集的交集，并将结果存储到destination
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public int SortedSetZInterStore(string destination, string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$11\r\nzinterstore\r\n${1}\r\n{2}\r\n${3}\r\n{4}\r\n", keys.Length + 3, RedisHelper.GetLength(destination), destination, keys.Length.ToString().Length, keys.Length);
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
        /// 返回指定区间内有序集合的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public List<string> SortedSetZRange(string key, int start, int end)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*4\r\n$6\r\nzrange\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(start.ToString()), start, RedisHelper.GetLength(end.ToString()), end)));
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
        /// 返回指定区间内有序集合的元素，带分数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public Dictionary<string,string> SortedSetZRangeWithscores(string key, int start, int end)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*5\r\n$6\r\nzrange\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n$10\r\nwithscores\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(start.ToString()), start, RedisHelper.GetLength(end.ToString()), end)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return new Dictionary<string, string>();
            //key值不存在
            if (result.StartsWith("*0"))
                return new Dictionary<string, string>();
            return RedisHelper.ClearString4(result);
        }
        /// <summary>
        /// 返回元素在有序集合中的排名(从小到大)
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">分数</param>
        /// <returns></returns>
        public int SortedSetZRank(string key, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$5\r\nzrank\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 返回元素在有序集合中的排名(从大到小)
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">分数</param>
        /// <returns></returns>
        public int SortedSetZRevRank(string key, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$8\r\nzrevrank\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 移除有序集合中的一个或多个元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">一个或多个元素</param>
        /// <returns></returns>
        public int SortedSetRem(string key, string[] values)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$4\r\nzrem\r\n${1}\r\n{2}\r\n", values.Length + 2, RedisHelper.GetLength(key), key);
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
        /// 获取有序集合指定元素的分数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">分数</param>
        /// <returns></returns>
        public string SortedSetZScore(string key, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$6\r\nzscore\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            if (result.StartsWith("$-1"))
                return string.Empty;
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return string.Empty;
            return RedisHelper.ClearString(result);
        }
        /// <summary>
        /// 返回多个有序集合的并集并存储在destination中
        /// </summary>
        /// <param name="destination">目标key</param>
        /// <param name="keys">多个key</param>
        /// <returns></returns>
        public int SortedSetZUnionStore(string destination, string[] keys)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$11\r\nzunionstore\r\n${1}\r\n{2}\r\n${3}\r\n{4}\r\n", keys.Length + 3, RedisHelper.GetLength(destination), destination, keys.Length.ToString().Length, keys.Length);
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
        /// 返回多个有序集合的并集并存储在destination中，针对每个集合可以增加权重，给分数值乘以对应的权重
        /// </summary>
        /// <param name="destination">目标key</param>
        /// <param name="keys">多个key</param>
        /// <param name="weights">权重，和数组keys里的元素是一一对应关系</param>
        /// <returns></returns>
        public int SortedSetZUnionStore(string destination, string[] keys, int[] weights)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$11\r\nzunionstore\r\n${1}\r\n{2}\r\n${3}\r\n{4}\r\n", keys.Length + 4 + weights.Length, RedisHelper.GetLength(destination), destination, keys.Length.ToString().Length, keys.Length);
            foreach (var item in keys)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(item), item);
            }
            sb.Append("$7\r\nweights\r\n");
            foreach (var item in weights)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", item.ToString().Length, item);
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
        public Dictionary<string,string> SortedSetScan(string key, string match)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*5\r\n$5\r\nzscan\r\n${0}\r\n{1}\r\n$1\r\n0\r\n$5\r\nmatch\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(match), match)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return new Dictionary<string, string>();
            //key值不存在
            if (result.StartsWith("*0"))
                return new Dictionary<string, string>();
            return RedisHelper.ClearString6(result);
        }
    }
}
