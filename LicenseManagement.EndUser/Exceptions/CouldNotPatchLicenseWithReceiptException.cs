using System;

namespace LicenseManagement.EndUser.Exceptions
{
    [Serializable]
	public class CouldNotPatchLicenseWithReceiptException : Exception
	{
		public CouldNotPatchLicenseWithReceiptException() { }
		public CouldNotPatchLicenseWithReceiptException(string message) : base(message) { }
		public CouldNotPatchLicenseWithReceiptException(string message, Exception inner) : base(message, inner) { }
		protected CouldNotPatchLicenseWithReceiptException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
