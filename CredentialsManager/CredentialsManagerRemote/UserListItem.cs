using Draeger.Testautomation.CredentialsManagerCore;
using Draeger.Testautomation.CredentialsManagerCore.Extensions;

namespace Draeger.Testautomation.CredentialsManagerRemote
{
    public class UserListItem
    {
        public string Username { get; set; }

        public UserGroup UserGroup { get; set; }

        public string DisplayName => ToString();

        public override string ToString()
        {
            return UserGroup.ToEnumString() + " " + Username;
        }
    }
}