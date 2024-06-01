using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core.Exceptions
{
    /// <summary>
    /// Cucumber中所有异常的基类，表示在Cucumber运行时发生的异常
    /// </summary>
    [Serializable]
    public class CoreException : Exception
    {
        /// <summary>
        /// 实例化一个异常
        /// </summary>
        public CoreException() { }
        /// <summary>
        /// 实例化一个异常，并指定其消息
        /// </summary>
        /// <param name="message"></param>
        public CoreException(string message) : base(message) { }
        /// <summary>
        /// 实例化一个异常，指定其消息和内部异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public CoreException(string message, Exception inner) : base(message, inner) { }
        protected CoreException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
