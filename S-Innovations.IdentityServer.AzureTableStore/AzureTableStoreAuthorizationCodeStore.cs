
using SInnovations.Azure.TableStorageRepository.TableRepositories;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Connect.Models;
using Thinktecture.IdentityServer.Core.Connect.Services;

namespace Thinktecture.IdentityServer.AzureTableStore
{
    public class TransientDataRepository<T> : ITransientDataRepository<T>
    {

        private readonly ITableRepository<T> repository;

        public TransientDataRepository(ITableRepository<T> repository)
        {
            this.repository = repository;
        }

        public Task StoreAsync(string key, T value)
        {
            repository.Add(value, key, "");
            return repository.SaveChangesAsync();
        }

        public async Task<T> GetAsync(string key)
        {
            var item = await repository.FindByKeysAsync(key, "");
            await repository.DeleteByKey(key, "");
            return item;

        }

        public Task RemoveAsync(string key)
        {
            repository.DeleteByKey(key, "");
            return repository.SaveChangesAsync();
        }
    }

    public class AzureTableStoreAuthorizationCodeStore : TransientDataRepository<AuthorizationCode>, IAuthorizationCodeStore
    {
        public AzureTableStoreAuthorizationCodeStore(IdentityServerTableContext context) : base(context.AuthorizationCodes)
        {
            
        }
        
    }
    public class AzureTableStoreTokenHandlerStore : TransientDataRepository<Token>, ITokenHandleStore
    {

        public AzureTableStoreTokenHandlerStore(IdentityServerTableContext context)
            : base(context.Tokens)
        {
            
        }
    }
}