using System;
using System.Runtime.Serialization;

namespace AlfredCrossChatLibrary
{
    [Serializable]
    internal class InvalidPlatformException : Exception
    {
        public InvalidPlatformException()
        {
        }

        public InvalidPlatformException(string message) : base(message)
        {
        }

        public InvalidPlatformException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidPlatformException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}