﻿using System;
using System.Security;
using Draeger.Testautomation.CredentialsManagerCore.Interfaces;

namespace Draeger.Testautomation.CredentialsManagerCore.Pooling
{
    public class PooledTestUserCredentials<T> : ITestUserCredentials where T: ITestUserCredentials, new()
    {
        private readonly ITestUserCredentials _credentials;
        private readonly Pool<ITestUserCredentials> _pool;

        private bool _disposedValue; // Dient zur Erkennung redundanter Aufrufe.

        public SecureString Username => _credentials.Username;

        public SecureString Password => _credentials.Password;

        public UserGroup UserGroup => _credentials.UserGroup;

        public PooledTestUserCredentials(Pool<ITestUserCredentials> pool)
        {
            this._pool = pool ?? throw new ArgumentNullException(nameof(pool));
            this._credentials = new T();
        }

        /// <summary>
        /// Dispose logic is wrapped inside this method to catch redundant calls to dispose
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                if (_pool.IsDisposed)
                {
                    _credentials.Dispose();
                }
                else
                {
                    _pool.Release(this);
                }
            }

            _disposedValue = true;
        }

        /// <summary>
        /// For complete disposable pattern
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }
}