using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.AzureTableStore
{
    public class AzureTableStoreConsentService : IConsentService
    {
        public Task<bool> RequiresConsentAsync(Client client, ClaimsPrincipal user, IEnumerable<string> scopes)
        {
            return Task.FromResult(client.RequireConsent);
        }

        public Task UpdateConsentAsync(Client client, ClaimsPrincipal user, IEnumerable<string> scopes)
        {
            return Task.FromResult<object>(null);
        }
    }
}
