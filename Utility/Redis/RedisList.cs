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
        /// 将一个或多个值插入列表头部，如果key不存在则会创建一个空列表后执行push操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">一个或多个value</param>
        /// <returns>列表长度</returns>
        public int ListLPush(string key, string[] values)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$5\r\nlpush\r\n${1}\r\n{2}\r\n", values.Length + 2, RedisHelper.GetLength(key), key);
            for (int i = 0, len = values.Length; i < len; i++)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(values[i]), values[i]);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 将一个或多个值插入列表尾部，如果key不存在则会创建一个空列表后执行push操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">一个或多个value</param>
        /// <returns>列表长度</returns>
        public int ListRPush(string key, string[] values)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n$5\r\nrpush\r\n${1}\r\n{2}\r\n", values.Length + 2, RedisHelper.GetLength(key), key);
            for (int i = 0, len = values.Length; i < len; i++)
            {
                sb.AppendFormat("${0}\r\n{1}\r\n", RedisHelper.GetLength(values[i]), values[i]);
            }
            socket.Send(Encoding.UTF8.GetBytes(sb.ToString()));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 将一个值插入已存在列表头部，并且返回列表长度。如果列表不存在则命令无效。只有当返回值大于零的时候才证明操作是成功的！
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public int ListLPushX(string key, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$6\r\nlpushx\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key),key, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 将一个值插入已存在列表尾部，并且返回列表长度。如果列表不存在则命令无效。只有当返回值大于零的时候才证明操作是成功的！
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public int ListRPushX(string key, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$6\r\nrpushx\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 通过索引查找值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public string ListLIndex(string key, int index)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*3\r\n$6\r\nlindex\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(index.ToString()), index)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return string.Empty;
            //key值不存在
            if (result.StartsWith("$-1"))
                return string.Empty;
            return RedisHelper.ClearString(result);
        }
        /// <summary>
        /// 获取列表长度
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public int ListLlenKey(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$4\r\nllen\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 根据参数 COUNT 的值，移除列表中与参数 VALUE 相等的元素。count 大于 0 : 从表头开始向表尾搜索，移除与 VALUE 相等的元素，数量为 COUNT 。count 小于 0 : 从表尾开始向表头搜索，移除与 VALUE 相等的元素，数量为 COUNT 的绝对值。count 等于 0 : 移除表中所有与 VALUE 相等的值。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public int ListLRem(string key,int count,string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*4\r\n$4\r\nlrem\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(count.ToString()), count, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 移除并获取列表的第一个元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string ListLPop(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$4\r\nlpop\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return string.Empty;
            //key值不存在
            if (result.StartsWith("$-1"))
                return string.Empty;
            return RedisHelper.ClearString(result);
        }
        /// <summary>
        /// 在pivot元素之前插入value元素。如果命令执行成功，返回插入操作完成之后，列表的长度。 如果没有找到指定元素 ，返回 -1 。 如果 key 不存在或为空列表，返回 0 。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pivot">参照元素</param>
        /// <param name="value">待插入值</param>
        /// <returns></returns>
        public int ListLInsertBefore(string key, string pivot, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*5\r\n$7\r\nlinsert\r\n${0}\r\n{1}\r\n$6\r\nbefore\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(pivot), pivot, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 在pivot元素之后插入value元素。如果命令执行成功，返回插入操作完成之后，列表的长度。 如果没有找到指定元素 ，返回 -1 。 如果 key 不存在或为空列表，返回 0 。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pivot">参照元素</param>
        /// <param name="value">待插入值</param>
        /// <returns></returns>
        public int ListLInsertAfter(string key, string pivot, string value)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*5\r\n$7\r\nlinsert\r\n${0}\r\n{1}\r\n$5\r\nafter\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, RedisHelper.GetLength(pivot), pivot, RedisHelper.GetLength(value), value)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return 0;
            return RedisHelper.ClearString5(result);
        }
        /// <summary>
        /// 返回列中中指定区间内的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public List<string> ListLRange(string key, int start, int end)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*4\r\n$6\r\nlrange\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, start.ToString().Length, start, end.ToString().Length, end)));
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
        /// 移除并获取列表的最后一个元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string ListRPop(string key)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*2\r\n$4\r\nrpop\r\n${0}\r\n{1}\r\n", RedisHelper.GetLength(key), key)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return string.Empty;
            //key值不存在
            if (result.StartsWith("$-1"))
                return string.Empty;
            return RedisHelper.ClearString(result);
        }
        /// <summary>
        /// 列表只保留区间内的元素，不在区间内的元素都被删除
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public bool ListLTrim(string key, int start, int end)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("*4\r\n$5\r\nltrim\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n", RedisHelper.GetLength(key), key, start.ToString().Length, start, end.ToString().Length, end)));
            string result = RedisHelper.Receive(socket);
            //key值已经表示其它类型了，所以无法执行命令
            if (result.StartsWith("-WRONGTYPE"))
                return false;
            return result.StartsWith("+OK");
        }
    }
}
