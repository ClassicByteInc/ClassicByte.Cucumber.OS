using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core.Exceptions
{

	[Serializable]
	public class UserAuthorizationException : Exception
	{
		public UserAuthorizationException(string message) : base(message) { }
		public UserAuthorizationException(string message, Exception inner) : base(message, inner) { }
		protected UserAuthorizationException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
