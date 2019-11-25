using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Draeger.Testautomation.CredentialsManagerCore.Interfaces;

namespace Draeger.Testautomation.CredentialsManagerCore.Pooling
{
    public abstract class UserCredentials<T> : ITestUserCredentials where T: ITestUserCredentials
    {
        // ReSharper disable once StaticMemberInGenericType
        private static int _count = -1;

        protected int Id;
        private bool _disposed;

        public SecureString Username { get; protected set; }

        public SecureString Password { get; protected set; }

        public UserGroup UserGroup { get; protected set; }

        protected UserCredentials(UserGroup userGroup)
        {
            UserGroup = userGroup;
            Id = Interlocked.Increment(ref _count);
            Init();
        }

        private void Init()
        { 
            Username = CredentialsManager.Instance.GetUsernameById(UserGroup, Id);
            Password = CredentialsManager.Instance.GetPasswordById(UserGroup, Id);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                Username.Dispose();
                Password.Dispose();
            }

            _disposed = true;
        }
    }
}