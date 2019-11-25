namespace Draeger.Testautomation.CredentialsManagerCore.Pooling.Users
{
    public class ServiceUserCredentials : UserCredentials<ServiceUserCredentials>
    {
        public ServiceUserCredentials() : base(UserGroup.Service)
        {
        }
    }
}