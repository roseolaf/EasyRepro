using System.Security;
using Draeger.Testautomation.CredentialsManagerCore.Interfaces;
using Draeger.Testautomation.CredentialsManagerCore.Pooling;
using Draeger.Testautomation.CredentialsManagerCore.Pooling.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Draeger.Testautomation.CredentialsManagerCore
{
    public class ManagedCredentials : ITestUserCredentials
    {
        private readonly ITestUserCredentials _credentials;

        #region IDisposable Support
        private bool _disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.
        private ILogger _logger;

        //make parameter-less constructor private to force usage of parameters for now
        private ManagedCredentials()
        { }

        public ManagedCredentials(ITestUserCredentials credentials, CredentialsManager owner, ILogger logger)
        {
            this._credentials = credentials;
            Owner = owner;
            _logger = logger;
        }

        public SecureString Username => _credentials.Username;

        public SecureString Password => _credentials.Password;
        public UserGroup UserGroup => _credentials.UserGroup;

        public CredentialsManager Owner { get; private set; } = null;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                Owner.ReturnCredentials(this, _logger);
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// This method is called by the owner's ReturnCredentials method when the reference count reaches zero
        /// </summary>
        internal void Release()
        {
            _credentials.Dispose();
        }

        #endregion

    }
}