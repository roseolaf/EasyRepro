using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Draeger.Testautomation.CredentialsManagerCore.Attributes;
using Draeger.Testautomation.CredentialsManagerCore.Bases;
using Draeger.Testautomation.CredentialsManagerCore.Exceptions;
using Draeger.Testautomation.CredentialsManagerCore.Extensions;
using Draeger.Testautomation.CredentialsManagerCore.Interfaces;
using Draeger.Testautomation.CredentialsManagerCore.Pooling;
using Draeger.Testautomation.CredentialsManagerCore.Pooling.Users;
using Microsoft.Azure.KeyVault.Models;
using Serilog;

namespace Draeger.Testautomation.CredentialsManagerCore
{
    public class CredentialsManager : Singleton<CredentialsManager>, IDisposable
    {
        private const string ContentTypePassword = "Password";

        private const string TagsKeyUserGroup = "UserGroup";

        private const string TagsKeyOriginalUsername = "UserName";

        private readonly object _padlock = new object();
        private readonly object _returnCredslock = new object();
        private readonly object _unusedCredslock = new object();
        private readonly object _getCredslock = new object();
        private readonly UserPool _pool;

        private readonly Dictionary<ManagedCredentials, CredentialsInfo> _usedCredentials =
            new Dictionary<ManagedCredentials, CredentialsInfo>();

        private bool _disposed;

        private bool _initialized;
        private KeyVaultConnector _kvc;

        private IXrmManagementHelper _xrmHelper;

        private CredentialsManager()
        {
            _pool = new UserPool(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }
 
       public void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _kvc?.Dispose();
            _disposed = true;
        }

        // ReSharper disable once UnusedMember.Global
        public void Init(IXrmManagementHelper helper)
        {
            _xrmHelper = helper;
            _kvc = new KeyVaultConnector();
            //TODO: remove workaround for not having command line arguments
            _kvc.Connect(Environment.GetEnvironmentVariable("akvClientId"),
                Environment.GetEnvironmentVariable("akvClientSecret"));
            _pool.Init();
            _initialized = true;
        }

        /// <summary>
        ///     This method will return a dictionary containing credentials assigned to the aliases as they were
        ///     requested via the CrmUserAttribute of the method given by logger<br/>
        ///     Usage: e.g. var users = CredentialsManager.Instance.GetCredentials(this, TestContext.TestName);
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public Dictionary<string, ManagedCredentials> GetCredentials(object caller, string testMethod,
            ILogger logger)
        {
            //#if DEBUG
            //            if (Environment.MachineName == "LDE7355")
            //            if (_usedCredentials.Count <= 0)
            //            {
            //                //reserve one user for other machine
            //                var user = GetUnusedCredentials(UserGroup.Sales, null);
            //                _usedCredentials.Add(user, new CredentialsInfo(){ExclusivelyUsed = true, SecurityRoles = new HashSet<SecurityRole>()});

            //            }
            //#endif

            logger.Debug("Test GetCredentials");
            var credentialsInfo = GetCallingMethodsCredentialsRequests(caller, testMethod);

            logger.Debug("Test credentialsInfo");
            var retVal = new Dictionary<string, ManagedCredentials>();
            foreach (var alias in credentialsInfo.Keys)
                //admin account is unmanaged!!
                if (credentialsInfo[alias].UserGroup == UserGroup.Admin)
                {
                    retVal.Add(alias, GetUnusedCredentials(credentialsInfo[alias].UserGroup, logger));

                    logger.Debug("Test GetUnusedCredentials Admin");
                }
                else
                {

                    //logger.Debug("Test GetCredentials before Credslock");
                    //lock (_getCredslock)
                    {

                        //logger.Debug("Test GetCredentials inside Credslock");
                        var matchingCredentials =
                            GetCredentialsUsingSecurityRoles(credentialsInfo[alias].SecurityRoles, logger) ??
                            PrepareUnusedCredentials(credentialsInfo[alias], logger);
                        retVal.Add(alias, matchingCredentials);
                    }

                }

            logger.Debug("Test GetCredentials end");
            return retVal;
        }

        // ReSharper disable once UnusedMember.Global
        public AdminUserCredentials GetAdminCredentials()
        {
            return _pool.AdminUser;
        }

        /// <summary>
        ///     This method fetches a set of unused credentials from the pool and assigns the requested security roles.
        ///     As this method changes the reference count, it is locked for thread safety
        /// </summary>
        /// <returns></returns>
        private ManagedCredentials PrepareUnusedCredentials(CredentialsInfo credentialsInfo, ILogger logger)
        {
            ManagedCredentials credentials;
            //lock (_unusedCredslock)
            {

                logger.Debug("Test PrepareUnusedCredentials start");
                credentials = GetUnusedCredentials(credentialsInfo.UserGroup, logger);
                credentialsInfo.ReferenceCount = 1;
                _usedCredentials.Add(credentials, credentialsInfo);
                SetSecurityRoles(credentials, credentialsInfo.SecurityRoles, logger, credentialsInfo.RemoveBasicRoles);

                logger.Debug("Test PrepareUnusedCredentials end");
            }

            return credentials;
        }

        private void SetSecurityRoles(ITestUserCredentials credentials, HashSet<SecurityRole> securityRoles, ILogger logger, bool removeBasicRoles)
        {
            if (!_initialized)
                throw new NotInitializedException(
                    "CredentialsManager has not been initialized with an IXrmManagementHelper!");
            _xrmHelper.SetSecurityRoles(credentials, securityRoles, logger, removeBasicRoles);
        }


        /// <summary>
        ///     This method resets the user roles assigned to a user to the standard user roles depending on the type of user
        /// </summary>
        /// <param name="managedCredentials"></param>
        /// <param name="securityRoles"></param>
        /// <param name="logger"></param>
        private void ResetUserRoles(ITestUserCredentials managedCredentials, HashSet<SecurityRole> securityRoles, ILogger logger)
        {
            if (!_initialized)
                throw new NotInitializedException(
                    "CredentialsManager has not been initialized with an IXrmManagementHelper!");
            _xrmHelper.ResetUserRoles(managedCredentials, securityRoles, logger);
        }

        /// <summary>
        ///     This method fetches a set of credentials from the user pool
        /// </summary>
        /// <returns></returns>
        private ManagedCredentials GetUnusedCredentials(UserGroup group, ILogger logger)
        {
            ITestUserCredentials cred;
            switch (@group)
            {
                case UserGroup.Sales:
                    cred = _pool.SalesUsers.Acquire();
                    break;
                case UserGroup.Service:
                    cred = _pool.ServiceUsers.Acquire();
                    break;
                case UserGroup.Admin:
                    cred = _pool.AdminUser;
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Pooled credentials don't exist for UserGroup {@group.ToString()}");
            }

            var manCred = new ManagedCredentials(cred, this, logger);
            return manCred;
        }

        /// <summary>
        ///     This method returns a set of credentials that use the requested set of security roles
        /// </summary>
        /// <param name="securityRoles"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private ManagedCredentials GetCredentialsUsingSecurityRoles(HashSet<SecurityRole> securityRoles, ILogger logger)
        {
            ManagedCredentials retVal;
            //lock (_padlock)
            {
                logger.Debug("Test GetCredentialsUsingSecurityRoles start");
                var credentials =
                    _usedCredentials.Where(x =>
                            !x.Value.ExclusivelyUsed && x.Value.SecurityRoles.SetEquals(securityRoles))
                        .Select(x => x.Key);
                var managedCredentials = credentials.ToList();
                if (!managedCredentials.Any()) return null;
                retVal = managedCredentials.First();
                logger.Information($"Reusing credentials of {retVal.Username.ToUnsecureString()}");
                Interlocked.Increment(ref _usedCredentials[retVal].ReferenceCount);

                logger.Debug("Test GetCredentialsUsingSecurityRoles end");
            }

            return retVal;
        }

        private static Dictionary<string, CredentialsInfo> GetCallingMethodsCredentialsRequests(object caller,
            string methodName)
        {
            var credentialsRequests = new Dictionary<string, CredentialsInfo>();
            var type = caller.GetType();
            var method = type.GetMethod(methodName);
            if (method == null)
                throw new ReflectionReturnedNullException(
                    $"Method name '{methodName}' did not match any known Method!");
            var userAttributes = method.GetCustomAttributes(typeof(CrmUserAttribute), false).Cast<CrmUserAttribute>();
            foreach (var userAttribute in userAttributes)
            {
                if (!credentialsRequests.Keys.Contains(userAttribute.UserId))
                    credentialsRequests.Add(userAttribute.UserId, new CredentialsInfo());

                credentialsRequests[userAttribute.UserId].ExclusivelyUsed = userAttribute.ExclusivelyRequested;
                credentialsRequests[userAttribute.UserId].UserGroup = userAttribute.UserGroup;
                credentialsRequests[userAttribute.UserId].SecurityRoles =
                    new HashSet<SecurityRole>(userAttribute.SecurityRoles);
                credentialsRequests[userAttribute.UserId].RemoveBasicRoles = userAttribute.RemoveBasicRoles;
            }

            return credentialsRequests;
        }

        /// <summary>
        ///     This method is responsible for the reference counting and release of unused credentials
        /// </summary>
        /// <param name="managedCredentials"></param>
        /// <param name="logger"></param>
        internal void ReturnCredentials(ManagedCredentials managedCredentials, ILogger logger)
        {
            //lock (_returnCredslock)
            {

                logger?.Debug($"ReturnCredentials start {managedCredentials.Username.ToUnsecureString()} {_usedCredentials[managedCredentials].ReferenceCount}");
                Interlocked.Decrement(ref _usedCredentials[managedCredentials].ReferenceCount);

                logger?.Debug("ReturnCredentials Decrement");
                if (_usedCredentials[managedCredentials].ReferenceCount >= 1) return;

                logger?.Debug("ReturnCredentials ReferenceCount");
                ResetUserRoles(managedCredentials, _usedCredentials[managedCredentials].SecurityRoles, logger);

                logger?.Debug("ReturnCredentials ResetUserRoles");
                managedCredentials.Release(logger);

                logger?.Debug("ReturnCredentials Release");
                _usedCredentials.Remove(managedCredentials);


                logger?.Debug("ReturnCredentials end");
            }
        }

        public SecureString GetUsernameById(UserGroup userGroup, int iD)
        {
            return GetUsernameById(_kvc, userGroup, iD);
        }

        public static SecureString GetUsernameById(KeyVaultConnector keyVaultConnector, UserGroup userGroup, int iD)
        {
            var secrets = keyVaultConnector.GetSecrets().Where(secret =>
                secret.ContentType == ContentTypePassword &&
                secret.Tags.ContainsKey(TagsKeyUserGroup) &&
                secret.Tags[TagsKeyUserGroup] == userGroup.ToEnumString()).ToList();
            secrets.Sort(Comparison);
            var name = secrets.ElementAt(iD).Tags[TagsKeyOriginalUsername];
            return name.ToSecureString();
        }

        private static int Comparison(SecretItem x, SecretItem y)
        {
            return string.Compare(x.Identifier.Name, y.Identifier.Name, StringComparison.Ordinal);
        }

        public SecureString GetPasswordById(UserGroup userGroup, int iD)
        {
            return GetPasswordById(_kvc, userGroup, iD);
        }

        public static SecureString GetPasswordById(KeyVaultConnector keyVaultConnector, UserGroup userGroup, int iD)
        {
            var secrets = keyVaultConnector.GetSecrets().Where(secret =>
                secret.ContentType == ContentTypePassword &&
                secret.Tags.ContainsKey(TagsKeyUserGroup) &&
                secret.Tags[TagsKeyUserGroup] == userGroup.ToEnumString()).ToList();
            secrets.Sort(Comparison);
            var name = secrets.ElementAt(iD).Identifier.Name;
            var bundle = keyVaultConnector.GetSecretAsync(name).Result;
            return bundle.Value.ToSecureString();
        }

        public int GetUserCount(UserGroup userGroup)
        {
            return GetUserCount(_kvc, userGroup);
        }

        public static int GetUserCount(KeyVaultConnector keyVaultConnector, UserGroup userGroup)
        {
            var secrets = keyVaultConnector.GetSecrets();
            var count = secrets.Count(secret =>
                secret.ContentType == ContentTypePassword &&
                secret.Tags.ContainsKey(TagsKeyUserGroup) &&
                secret.Tags[TagsKeyUserGroup] == userGroup.ToEnumString());
            return count;
        }

        // ReSharper disable once UnusedMember.Global
        public async Task AddUserCredentials(string username, string password, UserGroup userGroup)
        {
            using (var kvc = new KeyVaultConnector())
                await AddUserCredentials(kvc, username, password, userGroup);
        }

        public async Task AddUserCredentials(KeyVaultConnector keyVaultConnector, string username, string password,
            UserGroup userGroup)
        {
            var secretName = username.ToSecretName();
            await keyVaultConnector.SetSecretAsync(secretName, password,
                new Dictionary<string, string>
                {
                    {TagsKeyUserGroup, userGroup.ToEnumString()},
                    {TagsKeyOriginalUsername, username }
                }, ContentTypePassword).ConfigureAwait(false);
        }

        // ReSharper disable once UnusedMember.Global
        public async Task UpdateUserCredentials(string oldUsername, string newUsername = null, string password = null,
            UserGroup userGroup = UserGroup.Undefined)
        {
            await UpdateUserCredentials(_kvc, oldUsername, newUsername, password, userGroup).ConfigureAwait(false);
        }

        public async Task UpdateUserCredentials(KeyVaultConnector keyVaultConnector, string oldUsername,
            string newUsername = null, string password = null, UserGroup userGroup = UserGroup.Undefined)
        {
            if (string.IsNullOrEmpty(password))
            {
                var bundle = await keyVaultConnector.GetSecretAsync(oldUsername);
                password = bundle.Value;
            }

            if (newUsername != null)
            {
                var secrets = keyVaultConnector.GetSecrets();
                var secretName = secrets.First(secret => secret.Tags[TagsKeyOriginalUsername] == oldUsername).Id;
                await keyVaultConnector.DeleteSecretAsync(secretName);
            }

            await AddUserCredentials(keyVaultConnector, newUsername, password, userGroup);

        }

        // ReSharper disable once UnusedMember.Global
        public Task DeleteUserCredentials(string username)
        {
            using (var kvc = new KeyVaultConnector())
                return DeleteUserCredentials(kvc, username);
        }

        public Task DeleteUserCredentials(KeyVaultConnector keyVaultConnector, string username)
        {
            return keyVaultConnector.DeleteSecretAsync(username);
        }

        public Dictionary<string, UserGroup> GetUsers()
        {
            return GetUsers(_kvc);
        }

        public static Dictionary<string, UserGroup> GetUsers(KeyVaultConnector keyVaultConnector)
        {
            var secrets = keyVaultConnector.GetSecrets();
            var retVar = secrets
                .Where(s => s.Tags.ContainsKey(TagsKeyUserGroup) && s.ContentType == ContentTypePassword)
                .ToDictionary(secretItem => secretItem.Tags[TagsKeyOriginalUsername],
                    secretItem => secretItem.Tags[TagsKeyUserGroup].ToEnum<UserGroup>());
            return retVar;
        }

        private class CredentialsInfo
        {
            internal bool ExclusivelyUsed;
            internal bool RemoveBasicRoles;
            internal int ReferenceCount;
            internal HashSet<SecurityRole> SecurityRoles;
            internal UserGroup UserGroup;
        }
    }
}