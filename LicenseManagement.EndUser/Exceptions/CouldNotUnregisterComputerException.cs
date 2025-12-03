using System;

namespace Hymma.Lm.EndUser.Exceptions
{
    [Serializable]
	public class CouldNotUnregisterComputerException : Exception
	{
		public CouldNotUnregisterComputerException() { }
		public CouldNotUnregisterComputerException(string message) : base(message) { }
		public CouldNotUnregisterComputerException(string message, Exception inner) : base(message, inner) { }
		protected CouldNotUnregisterComputerException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
