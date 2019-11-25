namespace Draeger.Testautomation.CredentialsManagerCore.Pooling.Users
{
    public class AdminUserCredentials : UserCredentials<AdminUserCredentials>
    {
        public AdminUserCredentials() : base(UserGroup.Admin)
        {
        }
    }
}