using System;

namespace Hymma.Lm.EndUser.Exceptions
{
    [Serializable]
    public class ComputerNameException : Exception
    {
        public ComputerNameException() { }
        public ComputerNameException(string message) : base(message) { }
        public ComputerNameException(string message, Exception inner) : base(message, inner) { }
        protected ComputerNameException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
