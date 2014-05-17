using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using SInnovations.Identity.AzureTableStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;


namespace Thinktecture.IdentityServer.AzureTableStore
{
    public class AzureTableStoreServiceFactory
    {
        public static IdentityServerServiceFactory Create(
            string issuerUri, string siteName, string certificateThumbprint, 
            string storageConnectionString,
            string publicHostAddress = "")
        {
            Func<IdentityServerTableContext> storageContext = () =>  new IdentityServerTableContext(
                   CloudStorageAccount.Parse(storageConnectionString));
            Func<IdentityTableContext<IdentityUser>> identityContext = () =>
                new IdentityTableContext<IdentityUser>(CloudStorageAccount.Parse(storageConnectionString));

            var settings = new AzureTableStoreCoreSettings(storageContext(),
                issuerUri, siteName, certificateThumbprint, publicHostAddress);

            var consent = new AzureTableStoreConsentService();

            var logger = new TraceLogger();

            var fact = new IdentityServerServiceFactory
            {
                Logger = () => logger,
                UserService = () => UserServiceFactory.Factory<IdentityUser>(identityContext()),
                AuthorizationCodeStore = () => new AzureTableStoreAuthorizationCodeStore(storageContext()),
                TokenHandleStore = () => new AzureTableStoreTokenHandlerStore(storageContext()),
                CoreSettings = () => settings,
                ConsentService = () => consent
            };

            return fact;
        }
    }
}
