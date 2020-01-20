using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using TaADOLog.Logger;

namespace Draeger.Testautomation.CredentialsManagerCore.Interfaces
{
    public interface IXrmManagementHelper
    {
        void ResetUserRoles(ITestUserCredentials credentials, HashSet<SecurityRole> securityRoles, LoggerWrapper logger);
        void SetSecurityRoles(ITestUserCredentials credentials, HashSet<SecurityRole> securityRoles, LoggerWrapper logger, bool removeBasicRoles = false);
    }
}