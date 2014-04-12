using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Identity.AzureTableStorage
{
    public class IdentityUser<TKey, TLogin, TRole, TClaim> : TableEntity, IUser<TKey>
        where TLogin : IdentityUserLogin<TKey>
        where TRole : IdentityRole<TKey>
        where TClaim : IdentityUserClaim<TKey>
    {
        // Methods
        public IdentityUser()
        {
            this.Claims = new List<TClaim>();
            this.Roles = new List<TRole>();
            this.Logins = new List<TLogin>();
        }

        // Properties
        public virtual ICollection<TClaim> Claims { get; private set; }

        public virtual string Email { get; set; }

        public virtual bool EmailConfirmed { get; set; }

        public virtual TKey Id { get; set; }

        public virtual ICollection<TLogin> Logins { get; private set; }

        public virtual string PasswordHash { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }

        public virtual ICollection<TRole> Roles { get; private set; }

        public virtual string SecurityStamp { get; set; }

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual string UserName { get; set; }
    }


}
