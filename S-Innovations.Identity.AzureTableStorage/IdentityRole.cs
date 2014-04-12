using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Identity.AzureTableStorage
{
    public class IdentityRole<TKey> : TableEntity, IRole<TKey> //where TUserRole : IdentityUserRole<TKey>
    {
        // Methods
       // public IdentityRole()
      //  {
        //  this.Users = new List<TUserRole>();

    //    }
        // Properties
        public TKey Id { get; set; }
        public string Name { get; set; }
      //  public virtual ICollection<TUserRole> Users { get; private set; }
    }


}
