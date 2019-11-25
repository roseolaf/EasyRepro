using System;

namespace Draeger.Testautomation.CredentialsManagerCore.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CrmUserAttribute : Attribute
    {
        public string UserId { get; }
        public bool ExclusivelyRequested { get; set; }
        public UserGroup UserGroup { get; set; }
        public bool RemoveBasicRoles { get; set; } = false;
        public SecurityRole[] SecurityRoles { get; set; }

        public CrmUserAttribute(string userId, params SecurityRole[] securityRoles)
        {
            UserId = userId;
            SecurityRoles = securityRoles;
        }
    }
}