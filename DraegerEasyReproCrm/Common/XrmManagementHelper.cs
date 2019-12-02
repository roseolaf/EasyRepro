using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Draeger.CrmConnector.CrmOnline;
using Draeger.Testautomation.CredentialsManagerCore;
using Draeger.Testautomation.CredentialsManagerCore.Extensions;
using Draeger.Testautomation.CredentialsManagerCore.Interfaces;
using Infoman.Xrm.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Serilog;
using Draeger.Dynamics365.Testautomation.Common.EntityManager;

namespace Draeger.Dynamics365.Testautomation.Common
{
    public class XrmManagementHelper : IXrmManagementHelper
    {
        public void ResetUserRoles(ITestUserCredentials credentials, HashSet<SecurityRole> securityRoles, ILogger logger)
        {
            // Connect to the Organization service. 
            // The using statement assures that the service proxy is properly disposed.

            var organizationServiceProxy = CrmConnection.Instance.OpenConnection();
            ServiceContext context = CrmConnection.Instance.GetContext();

            var user = GetUserEntity(credentials, context);
            if (user == null) return;

            foreach (var role in securityRoles.Select(securityRole => FindSecurityRoleEntity(securityRole, organizationServiceProxy)))
            {
                logger.Information($"Role {role.Name} is retrieved for reset.");

                WithdrawSecurityRole(user, role, organizationServiceProxy);
                logger.Information($"Disassociated role {role.Name} from user {user.FirstName} {user.LastName}.\n");
            }
        }

        public void SetSecurityRoles(ITestUserCredentials credentials, HashSet<SecurityRole> securityRoles,
            ILogger logger, bool removeBasicRoles = false)
        {
            var organizationServiceProxy = CrmConnection.Instance.OpenConnection();
            var context = CrmConnection.Instance.GetContext();

            var user = GetUserEntity(credentials, context);

            if (removeBasicRoles)
            {
                foreach (var assignedRole in GetAssignedRoles(user, organizationServiceProxy))
                {
                    WithdrawSecurityRole(user, assignedRole, organizationServiceProxy);
                }
            }

            foreach (var role in securityRoles.Select(securityRole => FindSecurityRoleEntity(securityRole, organizationServiceProxy)))
            {
                logger.Information($"Role {role.Name} is retrieved for set.");
                if (!removeBasicRoles && IsUserInRole(user, role, organizationServiceProxy))
                //Find out if the role is already attached
                {
                    logger.Information($"Role {role.Name} is already associated to user {user.FirstName} {user.LastName}.\n");
                    continue;
                }

                if (role.Id == Guid.Empty || user.Id == Guid.Empty) continue;
                AssignSecurityRole(user, role, organizationServiceProxy);
                logger.Information(
                    $"Associated role {role.Name} to user {user.FirstName} {user.LastName}.\n");
            }

            context.Dispose();

        }

        private IEnumerable<Role> GetAssignedRoles(SystemUser user, IOrganizationService organizationServiceProxy)
        {
            var userid = user.Id;
            var qe = new QueryExpression("systemuserroles");
            qe.ColumnSet.AddColumns("systemuserid");
            qe.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, userid);

            var link1 = qe.AddLink("systemuser", "systemuserid", "systemuserid", JoinOperator.Inner);
            link1.Columns.AddColumns("fullname", "internalemailaddress");
            var link = qe.AddLink("role", "roleid", "roleid", JoinOperator.Inner);
            link.Columns.AddColumns("roleid", "name");
            return organizationServiceProxy.RetrieveMultiple(qe).Entities.Select(entity => entity.ToEntity<Role>());
        }

        private static void AssignSecurityRole(SystemUser user, Role role, OrganizationServiceProxy organizationServiceProxy)
        {
            organizationServiceProxy.Associate(
                "systemuser",
                user.Id,
                new Relationship("systemuserroles_association"),
                new EntityReferenceCollection { role.ToEntityReference() });
        }

        private static void WithdrawSecurityRole(SystemUser user, Role role, OrganizationServiceProxy organizationServiceProxy)
        {
            organizationServiceProxy.Disassociate(
                "systemuser",
                user.Id,
                new Relationship("systemuserroles_association"),
                new EntityReferenceCollection { new EntityReference("role", role.Id) });
        }

        private static SystemUser GetUserEntity(ITestUserCredentials credentials, ServiceContext context)
        {
            var user = (from sysUser in context.SystemUserSet
                        where sysUser.DomainName.Equals(credentials.Username.ToUnsecureString())
                        select sysUser).FirstOrDefault();
            return user;
        }

        private static bool IsUserInRole(SystemUser user, Role role, OrganizationServiceProxy organizationServiceProxy)
        {
            var systemUserLink = new LinkEntity()
            {
                LinkFromEntityName = SystemUserRoles.EntityLogicalName,
                LinkFromAttributeName = "systemuserid",
                LinkToEntityName = SystemUser.EntityLogicalName,
                LinkToAttributeName = "systemuserid",
                LinkCriteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(
                            "systemuserid", ConditionOperator.Equal, user.Id)
                    }
                }
            };

            // Build the query.
            var linkQuery = new QueryExpression()
            {
                EntityName = Role.EntityLogicalName,
                ColumnSet = new ColumnSet("roleid"),
                LinkEntities =
                {
                    new LinkEntity()
                    {
                        LinkFromEntityName = Role.EntityLogicalName,
                        LinkFromAttributeName = "roleid",
                        LinkToEntityName = SystemUserRoles.EntityLogicalName,
                        LinkToAttributeName = "roleid",
                        LinkEntities = {systemUserLink}
                    }
                },
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("roleid", ConditionOperator.Equal, role.Id)
                    }
                }
            };

            // Retrieve matching roles.
            var matchEntities = organizationServiceProxy.RetrieveMultiple(linkQuery);

            // if an entity is returned then the user is a member
            // of the role
            var isUserInRole = (matchEntities.Entities.Count > 0);
            return isUserInRole;
        }

        private static Role FindSecurityRoleEntity(SecurityRole securityRole, OrganizationServiceProxy organizationServiceProxy)
        {
            var givenRole = securityRole.ToEnumString();
            // see https://docs.microsoft.com/de-de/dynamics365/customer-engagement/developer/sample-associate-security-role-user for details
            var query = new QueryExpression
            {
                EntityName = Role.EntityLogicalName,
                ColumnSet = new ColumnSet("roleid", "name"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "name",
                            Operator = ConditionOperator.Equal,
                            Values = {givenRole}
                        }
                    }
                }
            };

            var roles = organizationServiceProxy.RetrieveMultiple(query);
            Role currentRole = null;
            if (roles.Entities.Count > 0)
            {
                currentRole = roles.Entities.First().ToEntity<Role>();
            }

            return currentRole;
        }
    }
}