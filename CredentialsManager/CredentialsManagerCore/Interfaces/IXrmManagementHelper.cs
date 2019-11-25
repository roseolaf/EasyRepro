using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Draeger.Testautomation.CredentialsManagerCore.Interfaces
{
    public interface IXrmManagementHelper
    {
        void ResetUserRoles(ITestUserCredentials credentials, HashSet<SecurityRole> securityRoles, ILogger logger);
        void SetSecurityRoles(ITestUserCredentials credentials, HashSet<SecurityRole> securityRoles, ILogger logger, bool removeBasicRoles = false);
    }
}