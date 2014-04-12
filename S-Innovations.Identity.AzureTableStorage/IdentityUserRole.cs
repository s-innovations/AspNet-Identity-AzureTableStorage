using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Identity.AzureTableStorage
{
    public class IdentityUserRole<TKey> : TableEntity
    {

        // Properties
        public virtual TKey RoleId { get; set; }
        public virtual TKey UserId { get; set; }
    }

 

}
