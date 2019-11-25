using System;
using System.Runtime.Serialization;

namespace Draeger.Testautomation.CredentialsManagerCore
{
    [DataContract]
    public enum UserGroup
    {
        [EnumMember(Value="adm")]
        Admin,
        [EnumMember(Value = "svc")]
        Service,
        [EnumMember(Value = "sal")]
        Sales,
        [EnumMember(Value="und")]
        Undefined
    }
}