using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Redis
{
    public class RedisException : Exception
    {
        private readonly string error;
        private readonly Exception innerException;
        public RedisException(string msg) : base(msg)
        {
            this.error = "[RedisClient]" + msg;
        }
        public RedisException(string msg, Exception innerException) : base(msg)
        {
            this.innerException = innerException;
            this.error = "[RedisClient]" + msg;
        }
        public string GetError()
        {
            return error;
        }
    }
}
