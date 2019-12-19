using System;
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

        //make parameter-less constructor private to force usage of parameters for now
        private ManagedCredentials()
        { }

        public ManagedCredentials(ITestUserCredentials credentials, CredentialsManager owner)
        {
            this._credentials = credentials;
            Owner = owner;
        }

        public SecureString Username => _credentials.Username;

        public SecureString Password => _credentials.Password;
        public UserGroup UserGroup => _credentials.UserGroup;

        public CredentialsManager Owner { get; private set; } = null;

        protected virtual void Dispose(bool disposing, ILogger logger)
        {
            //_logger.Debug($"Managed Credentials before dispose {_disposedValue}");
            //if (_disposedValue) return;
            if (disposing)
            {

                logger?.Debug("Managed Credentials dispose");
                Owner.ReturnCredentials(this, logger);
            }

            //_disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true, null);
        }

        public void Return(ILogger logger)
        {
            Dispose(true, logger);
        }

        /// <summary>
        /// This method is called by the owner's ReturnCredentials method when the reference count reaches zero
        /// </summary>
        /// <param name="logger"></param>
        internal void Release(ILogger logger)
        {
            logger?.Debug("ManagedCredentials.Release");
            switch (_credentials.UserGroup)
            {
                case UserGroup.Sales:
                    ((PooledTestUserCredentials<SalesUserCredentials>)_credentials).Dispose(true, logger);
                    break;
                case UserGroup.Admin:
                    ((PooledTestUserCredentials<AdminUserCredentials>)_credentials).Dispose(true, logger);
                    break;
                case UserGroup.Service:
                    ((PooledTestUserCredentials<ServiceUserCredentials>)_credentials).Dispose(true, logger);
                    break;
                case UserGroup.Undefined:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

    }
}