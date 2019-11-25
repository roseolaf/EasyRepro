using System;
using System.Security;

namespace Draeger.Testautomation.CredentialsManagerCore.Interfaces
{
    public interface ITestUserCredentials : IDisposable
    {
        SecureString Username { get; }

        SecureString Password { get; }

        UserGroup UserGroup { get; }
    }
}