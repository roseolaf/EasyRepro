using System;
using System.Runtime.Serialization;

namespace Draeger.Testautomation.CredentialsManagerCore.Exceptions
{
    [Serializable]
    public class NotConnectedException : CredentialsManagerExceptionBase
    {
        public NotConnectedException()
        {
        }

        public NotConnectedException(string message) 
            : base(message)
        {
        }

        public NotConnectedException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected NotConnectedException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}