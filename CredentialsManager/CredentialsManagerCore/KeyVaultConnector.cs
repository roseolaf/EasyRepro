using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Draeger.Testautomation.CredentialsManagerCore.CommandLine;
using Draeger.Testautomation.CredentialsManagerCore.Exceptions;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Draeger.Testautomation.CredentialsManagerCore
{
    public class KeyVaultConnector: IDisposable
    {
        private KeyVaultClient _client;
        private bool _disposedValue;
        private string _username;

        public KeyVaultConnector()
        {
            var args = new Arguments();
            ClientId = args["secretName"];
            ClientSecret = args["password"];
        }

        // ReSharper disable once UnusedMember.Global
        public KeyVaultConnector(string endpoint, string clientId, string clientSecret)
        {
            Endpoint = endpoint;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public string Endpoint { get; set; } = "https://testautomation-vault.vault.azure.net/";

        public string ClientId { get; set; }

        public string ClientSecret { private get; set; }

        /// <summary>
        /// Connects to Azure Key Vault using the command line ClientID and ClientSecret of an authorized Service principal
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void Connect()
        {
            if (string.IsNullOrEmpty(ClientId)) throw new ArgumentException("ClientId has not been set!");
            if (string.IsNullOrEmpty(ClientSecret)) throw new ArgumentException("ClientSecret has not been set!");
            Connect(ClientId, ClientSecret);
        }

        /// <summary>
        /// Connects to Azure Key vault using the provided ClientID and ClientSecret of an authorized Service principal
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public void Connect(string clientId, string clientSecret)
        {
            ICredentials credentials = new NetworkCredential("tmp-QA-TA-001","DraegerQA01");
            var webProxy = new WebProxy {Address = new Uri("http://185.46.212.92:80"), Credentials = credentials};
            var httpClientHandler = new WinHttpHandler()
            {
                WindowsProxyUsePolicy = WindowsProxyUsePolicy.UseCustomProxy,
                Proxy = webProxy,
                SendTimeout = TimeSpan.FromSeconds(120),
                ReceiveDataTimeout = TimeSpan.FromSeconds(120),
                ReceiveHeadersTimeout = TimeSpan.FromSeconds(120),

            };
            var httpClient = new HttpClient(httpClientHandler);
            _client = new KeyVaultClient(async (authority, resource, scope) =>
            {
                var adCredential = new ClientCredential(clientId, clientSecret);
                var authenticationContext = new AuthenticationContext(authority, null);
                return (await authenticationContext.AcquireTokenAsync(resource, adCredential).ConfigureAwait(false))
                    .AccessToken;
            },httpClient: httpClient);

        }

        public void Disconnect()
        {
            _client.Dispose();
        }

        public async Task<SecretBundle> GetSecretAsync(string secretName)
        {
            if (_client == null) throw new NotConnectedException("Not connected to Azure key vault!");
            if (string.IsNullOrEmpty(Endpoint)) throw new ArgumentException("Endpoint has not been set!");
            if (string.IsNullOrEmpty(secretName)) throw new ArgumentException(nameof(secretName));

            var keyVaultSecret = await _client.GetSecretAsync(Endpoint, secretName).ConfigureAwait(false);
            return keyVaultSecret;
        }

        public async Task<bool> SetSecretAsync(string secretName, string value, IDictionary<string,string> tags = null, string contentType = null, SecretAttributes secretAttributes = null)
        {
            if (_client == null) throw new NotConnectedException("Not connected to Azure key vault!");
            if (string.IsNullOrEmpty(Endpoint)) throw new ArgumentException("Endpoint has not been set!");
            if (string.IsNullOrEmpty(secretName)) throw new ArgumentException(nameof(secretName));
            if (string.IsNullOrEmpty(value)) throw new ArgumentException(nameof(value));
            await _client.SetSecretAsync(Endpoint, secretName, value, tags, contentType, secretAttributes)
                .ConfigureAwait(false);

            return true;
        }
        public async Task DeleteSecretAsync(string secretName)
        {
            if (_client == null) throw new NotConnectedException("Not connected to Azure key vault!");
            if (string.IsNullOrEmpty(Endpoint)) throw new ArgumentException("Endpoint has not been set!");
            if (string.IsNullOrEmpty(secretName)) throw new ArgumentException(nameof(secretName));
            await _client.DeleteSecretAsync(Endpoint, secretName).ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing) Disconnect();
            _disposedValue = true;
        }

        public List<SecretItem> GetSecrets()
        {
            if (_client == null) throw new NotConnectedException("Not connected to Azure key vault!");
            var secrets = Task.Run(async () => await _client.GetSecretsAsync(Endpoint)).Result;
            return secrets.ToList();
        }

    }
}