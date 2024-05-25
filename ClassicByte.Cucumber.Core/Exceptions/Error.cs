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
        protected Error(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
