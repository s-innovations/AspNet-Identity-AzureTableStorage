using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SInnovations.Identity.AzureTableStorage
{
    public class IdentityUserLogin<TKey> 
    {
    
        // Properties
        public virtual string LoginProvider { get; set; }
        public virtual string ProviderKey { get; set; }
        public virtual TKey UserId { get; set; }
    }

 

}
