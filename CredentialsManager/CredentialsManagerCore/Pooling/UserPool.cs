using System.Threading.Tasks;
using Draeger.Testautomation.CredentialsManagerCore.Bases;
using Draeger.Testautomation.CredentialsManagerCore.Interfaces;
using Draeger.Testautomation.CredentialsManagerCore.Pooling.Users;

namespace Draeger.Testautomation.CredentialsManagerCore.Pooling
{
    namespace Pooling
    {
    }

    /// <summary>
    /// This class provides access to a pool of Draeger Test CRM users with either service or sales role <br/>
    /// usage: <br/>using(ITestUserCredentials salesCredentials = UserPool.Instance.SalesUsers.Acquire()) <br/>
    /// {<br/>//do stuff<br/>}
    /// </summary>
    public sealed class UserPool 
    {
        private readonly CredentialsManager _owner;


        public UserPool(CredentialsManager owner)
        {
            _owner = owner;
        }

        internal async void Init()
        {
            var serviceUserCount = _owner.GetUserCount(UserGroup.Service);
            var salesUserCount =  _owner.GetUserCount(UserGroup.Sales);
            ServiceUsers = new Pool<ITestUserCredentials>(serviceUserCount, p => new PooledTestUserCredentials<ServiceUserCredentials>(p),
                LoadingMode.Lazy, AccessMode.Fifo);
            SalesUsers = new Pool<ITestUserCredentials>(salesUserCount, p => new PooledTestUserCredentials<SalesUserCredentials>(p),
                LoadingMode.Lazy, AccessMode.Fifo);
            AdminUser = new AdminUserCredentials();
        }

        public Pool<ITestUserCredentials> ServiceUsers { get; private set; }

        public Pool<ITestUserCredentials> SalesUsers { get; private set; }

        public AdminUserCredentials AdminUser { get; private set; }
    }
}
