namespace Draeger.Testautomation.CredentialsManagerCore.Pooling.Users
{
    public class SalesUserCredentials : UserCredentials<SalesUserCredentials>
    {
        public SalesUserCredentials() : base(UserGroup.Sales)
        {
        }
    }
}