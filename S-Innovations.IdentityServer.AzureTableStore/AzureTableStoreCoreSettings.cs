
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.AzureTableStore
{
    public class AzureTableStoreCoreSettings : ICoreSettings
    {
        private readonly IdentityServerTableContext context;
        private string _issuerUri;
        private string _siteName;
        private X509Certificate2 _certificate;
        private string _publicHostAddress;

        public AzureTableStoreCoreSettings(IdentityServerTableContext context, string issuerUri, string siteName, string certificateThumbprint, string publicHostAddress)
        {
            _issuerUri = issuerUri;
            _siteName = siteName;
            _certificate = X509.LocalMachine.My.Thumbprint.Find(certificateThumbprint, false).First();
            _publicHostAddress = publicHostAddress;        
            this.context = context;
        }
        public Task<IEnumerable<Scope>> GetScopesAsync()
        {
            var query = from scope in context.Scopes
                        select scope;
            return Task.FromResult(query.AsEnumerable());
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return context.Clients.FindByKeysAsync(clientId, "");
        }

        public X509Certificate2 GetSigningCertificate()
        {
            return _certificate;
        }

        public string GetIssuerUri()
        {
            return _issuerUri;
        }

        public string GetSiteName()
        {
            return _siteName;
        }

        public string GetPublicHost()
        {
            return _publicHostAddress;
        }

        public InternalProtectionSettings GetInternalProtectionSettings()
        {
            var settings = new InternalProtectionSettings
            {
                Issuer = GetIssuerUri(),
                Audience = "internal",
                SigningKey = "jKhUkbfzz4IqMTo66J6GATNgOWqA38SFNMCo/FR1Yhs=",
                Ttl = 60
            };

            return settings;
        }
    }
}
