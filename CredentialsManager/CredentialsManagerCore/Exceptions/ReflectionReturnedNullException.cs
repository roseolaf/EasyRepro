using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draeger.Testautomation.CredentialsManagerCore.Exceptions
{
    public class ReflectionReturnedNullException : CredentialsManagerExceptionBase
    {
        public ReflectionReturnedNullException(string message)
            : base(message)
        {
        }
    }
}
