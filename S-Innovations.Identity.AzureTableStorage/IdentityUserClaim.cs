using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SInnovations.Identity.AzureTableStorage
{
    public class IdentityUserClaim<TKey> : TableEntity
    {
      
        // Properties
        public virtual string ClaimType { get; set; }
        public virtual string ClaimValue { get; set; }
        public virtual int Id { get; set; }
        public virtual TKey UserId { get; set; }
    }

 

}
