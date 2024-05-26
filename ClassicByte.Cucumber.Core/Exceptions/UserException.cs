using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core.Exceptions
{

	[Serializable]
	public class UserException : CoreException
	{
		public UserException() { }
		public UserException(string message) : base(message) { }
		public UserException(string message, Exception inner) : base(message, inner) { }
		protected UserException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
