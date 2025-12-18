using System;

namespace LicenseManagement.EndUser.Exceptions
{

    [Serializable]
	public class ComputerOfflineException : Exception
	{
		public ComputerOfflineException() : base("Computer is not connected to Internet.") { }
		public ComputerOfflineException(string message, Exception inner) : base(message, inner) { }
		protected ComputerOfflineException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
