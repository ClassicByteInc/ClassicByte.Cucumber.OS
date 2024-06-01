using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core.Exceptions
{

    /// <summary>
    /// Cucumber遇到的无法处理的错误类
    /// </summary>
    [Serializable]
    public class Error : Exception
    {
        /// <summary>
        /// 实例化一个错误
        /// </summary>
        public Error() { }

        /// <summary>
        /// 实例化一个错误，并指定消息和错误代码
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exitCode"></param>
        public Error(string message, String exitCode) : base(message) { ErrorCode = exitCode; }

        /// <summary>
        /// 实例化一个错误,并指定其消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exitCode"></param>
        /// <param name="inner"></param>
        public Error(string message, String exitCode, Exception inner) : base(message, inner) { }

        /// <summary>
        /// 当前错误的错误代码
        /// </summary>
        public String ErrorCode { get; set; }

        /// <summary>
        /// 向控制台输出错误信息,或者返回错误信息的字符串
        /// </summary>
        public String Print()
        {
            Console.WriteLine($"Cucumber遇到问题,正在收集信息...\n\n错误消息:{Message}\n错误代码:{ErrorCode}\n内部错误:{$"[{InnerException.GetType()}]{InnerException.Message}"}\n\n源:{Source}\n{ToString()}", ConsoleColor.Red);
            return $"Cucumber遇到问题,正在收集信息...\n\n错误消息:{Message}\n错误代码:{ErrorCode}\n内部错误:{$"[{InnerException.GetType()}]{InnerException.Message}"}\n\n源:{Source}\n{ToString()}";
        }
        protected Error(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
