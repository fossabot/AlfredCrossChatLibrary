using System;
using System.Runtime.Serialization;

namespace AlfredCrossChatLibrary
{
    [Serializable]
    internal class ChatConfigurationException : Exception
    {
        public ChatConfigurationException()
        {
        }

        public ChatConfigurationException(string message) : base(message)
        {
        }

        public ChatConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ChatConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}