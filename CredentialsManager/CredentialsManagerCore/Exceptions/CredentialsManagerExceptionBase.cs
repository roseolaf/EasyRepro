using System;
using System.Runtime.Serialization;

namespace Draeger.Testautomation.CredentialsManagerCore.Exceptions
{
    public class CredentialsManagerExceptionBase : Exception
    {
        public CredentialsManagerExceptionBase(string message) : base(message)
        {
        }

        public CredentialsManagerExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CredentialsManagerExceptionBase(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }

        protected CredentialsManagerExceptionBase()
        {
            throw new NotImplementedException();
        }
    }
}