using System;

namespace LicenseManagement.EndUser.Exceptions
{

    [Serializable]
	public class CouldNotReadLicenseFromDiskException : Exception
	{
		public CouldNotReadLicenseFromDiskException() { }
		public CouldNotReadLicenseFromDiskException(string message) : base(message) { }
		public CouldNotReadLicenseFromDiskException(string message, Exception inner) : base(message, inner) { }
		protected CouldNotReadLicenseFromDiskException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
