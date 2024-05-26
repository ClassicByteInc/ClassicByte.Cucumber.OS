using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core.Exceptions
{

	[Serializable]
	public class LogonFailureException : UserException
	{
		public LogonFailureException() { }
		public LogonFailureException(string message) : base(message) { }
		public LogonFailureException(string message, Exception inner) : base(message, inner) { }
		protected LogonFailureException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
