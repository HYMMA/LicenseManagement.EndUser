using System;

namespace Hymma.Lm.EndUser.Exceptions
{
    internal class CouldNotWriteComputerIdException:Exception
    {
        public CouldNotWriteComputerIdException() { }
        public CouldNotWriteComputerIdException(string message="Could not write computer Id to any of the registry keys.") : base(message) { }
        public CouldNotWriteComputerIdException(string message, Exception inner) : base(message, inner) { }
        protected CouldNotWriteComputerIdException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
