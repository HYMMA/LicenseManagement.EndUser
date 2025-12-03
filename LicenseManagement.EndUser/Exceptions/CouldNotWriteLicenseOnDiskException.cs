using System;

namespace Hymma.Lm.EndUser.Exceptions
{
    public class CouldNotWriteLicenseOnDiskException : Exception
    {
        public CouldNotWriteLicenseOnDiskException() { }
        public CouldNotWriteLicenseOnDiskException(string message) : base(message) { }
        public CouldNotWriteLicenseOnDiskException(string message, Exception inner) : base(message, inner) { }
        protected CouldNotWriteLicenseOnDiskException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
