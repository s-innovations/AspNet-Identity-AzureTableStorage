using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SInnovations.Azure.TableStorageRepository;
using SInnovations.Azure.TableStorageRepository.DataInitializers;
using SInnovations.Azure.TableStorageRepository.TableRepositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Thinktecture.IdentityServer.Core.Connect.Models;
using Thinktecture.IdentityServer.Core.Models;

namespace Thinktecture.IdentityServer.AzureTableStore
{
    public class IdentityServerTableContext : TableStorageContext
    {
        static IdentityServerTableContext()
        {
            Table.SetInitializer(new CreateTablesIfNotExists<IdentityServerTableContext>());

        }
        public IdentityServerTableContext(CloudStorageAccount account) :base(account)
        {

        }

        
        private static Task<ClaimsPrincipal> SubjectDeserializer(EntityProperty property)
        {
            return Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity(
                 JArray.Parse(property.StringValue)
                 .Select(j => new Claim(j["Type"].ToObject<string>(), j["Value"].ToObject<string>())))));
        }
        private static Task<EntityProperty> SubjectSerializer(ClaimsPrincipal subject)
        {            
            return Task.FromResult(new EntityProperty(
                JsonConvert.SerializeObject(subject.Claims.Select(c => new { c.Type, c.Value }))));
        }
        protected override void OnModelCreating(TableStorageModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Scope>()
                .HasKeys(s => s.Name, s => "")
                .WithPropertyOf(s => s.Claims)
                .ToTable("Scopes");

            modelbuilder.Entity<Client>()
                .HasKeys(c=>c.ClientId,c=>"")
                .WithEnumProperties()
                .WithUriProperties()
                .WithPropertyOf(c=>c.RedirectUris)
                .WithPropertyOf(c => c.ScopeRestrictions)
                .ToTable("Clients");

            modelbuilder.Entity<AuthorizationCode>()
                .WithPropertyOf(code => code.Subject,SubjectDeserializer,SubjectSerializer)
                .WithPropertyOf(code => code.Client,
                    (p)=> this.Clients.FindByKeysAsync(p.StringValue,""),
                    p=>Task.FromResult(new EntityProperty(p.ClientId)))
                .WithPropertyOf(code=>code.RequestedScopes)
                .WithUriProperties()
                .ToTable("AuthorizationCodes");

            modelbuilder.Entity<Token>()
                .WithNoneDefaultConstructor((props) => new Token(props["Type"].StringValue))
                .WithPropertyOf(token => token.Client,
                    (p) => this.Clients.FindByKeysAsync(p.StringValue, ""),
                    p => Task.FromResult(new EntityProperty(p.ClientId)))
                    .WithPropertyOf(token=>token.Claims)
                .ToTable("Tokens");

            base.OnModelCreating(modelbuilder);
        }

        public virtual ITableRepository<Scope> Scopes { get; set; }
        public virtual ITableRepository<Client> Clients { get; set; }
        public virtual ITableRepository<AuthorizationCode> AuthorizationCodes { get; set; }
        public virtual ITableRepository<Token> Tokens { get; set; }

    }
}
