using System;
using System.Collections.Generic;
using System.Threading;
using Draeger.Testautomation.CredentialsManagerCore;
using Draeger.Testautomation.CredentialsManagerCore.Extensions;
using Draeger.Testautomation.CredentialsManagerCore.Interfaces;

namespace CredentialsManagerCoreTests.Mocks
{
    public class XrmManagementHelperMock : IXrmManagementHelper
    {
        public void ResetUserRoles(ITestUserCredentials credentials, HashSet<SecurityRole> securityRoles)
        {
            Console.WriteLine($"{DateTime.Now}: Resetting user roles for user {credentials.Username.ToUnsecureString()}...");
            Thread.Sleep(100);
            Console.WriteLine($"{DateTime.Now}: User roles for user {credentials.Username.ToUnsecureString()} were reset!");
        }

        public void SetSecurityRoles(ITestUserCredentials credentials, HashSet<SecurityRole> securityRoles)
        {
            Console.WriteLine($"{DateTime.Now}: Setting user roles for user {credentials.Username.ToUnsecureString()}...");
            Thread.Sleep(100);
            Console.WriteLine($"{DateTime.Now}: User roles for user {credentials.Username.ToUnsecureString()} were set!");
        }
    }
}