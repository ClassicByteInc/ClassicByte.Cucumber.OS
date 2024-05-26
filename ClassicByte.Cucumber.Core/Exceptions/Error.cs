using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core.Exceptions
{

    [Serializable]
    public class Error : Exception
    {
        public Error() { }
        public Error(string message, String exitCode) : base(message) { ErrorCode = exitCode; }
        public Error(string message, Exception inner) : base(message, inner) { }
        public String ErrorCode { get; set; }
        public void Print()
        {
            Console.WriteLine($"Cucumber遇到问题,正在收集信息...\n\n错误消息:{Message}\n错误代码:{ErrorCode}\n内部错误:{$"[{InnerException.GetType()}]{InnerException.Message}"}\n\n源:{Source}\n{ToString()}",ConsoleColor.Red);
        }
        protected Error(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
