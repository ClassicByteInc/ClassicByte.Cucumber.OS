using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core.Exceptions
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class CoreException : Exception
	{
		public CoreException() { }
		public CoreException(string message) : base(message) { }
		public CoreException(string message, Exception inner) : base(message, inner) { }
		protected CoreException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
