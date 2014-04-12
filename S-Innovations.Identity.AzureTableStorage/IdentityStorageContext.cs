using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage;
using SInnovations.Azure.TableStorageRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Identity.AzureTableStorage
{
    public class IdentityRole : IdentityRole<string>
    {
        // Methods
        public IdentityRole()
        {
            base.Id = Guid.NewGuid().ToString();
        }

        public IdentityRole(string roleName) : this()
        {
            base.Name = roleName;
        }
    }


    public class IdentityUserRole : IdentityUserRole<string>
    {
    }



    public class IdentityUserClaim : IdentityUserClaim<string>
    {

    }


    public class IdentityUserLogin : IdentityUserLogin<string>
    {

    }
    public class IdentityUser : IdentityUser<string, IdentityUserLogin, IdentityRole, IdentityUserClaim>, IUser
    {
        // Methods
        public IdentityUser()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public IdentityUser(string userName) : this()
        {
            this.UserName = userName;
        }
    }


    public class IdentityTableContext<TUser> : IdentityTableContext<TUser,IdentityRole,string,IdentityUserLogin,IdentityUserClaim>
        where TUser : IdentityUser
    {
        public IdentityTableContext(CloudStorageAccount account ) : base(account)
        {

        }
    }

    public class IdentityTableContext<TUser, TRole, TKey, TUserLogin, TUserClaim> : TableStorageContext
        where TUser : IdentityUser<TKey, TUserLogin,TRole, TUserClaim>
        where TRole : IdentityRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
       // where TUserRole : IdentityUserRole<TKey>, new()
        where TUserClaim : IdentityUserClaim<TKey>,new()
    {

        public IdentityTableContext(CloudStorageAccount account) : base(account)
        {

        }

        protected override void OnModelCreating(TableStorageModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TUser>()
                .HasKeys(u=>u.Id,u=>"")
                .WithIndex(u => u.UserName)
                .WithIndex(u=>u.Email)
                .WithCollectionOf<TUserClaim>(u=>u.Claims,(source, user) => (from claim in source where claim.PartitionKey == user.Id.ToString() select claim))
                .ToTable("AspNetUsers");

            modelBuilder.Entity<TRole>()
                .HasKeys(r=>r.Id,r=>r.Name)                
                //RowKey of TUserRole is RoleID, Consider if its better to have one partition per role.
         //       .WithCollectionOf(r=>r.Users, (source,role) => from ent in source where ent.RowKey == role.Id.ToString() select ent)
                .ToTable("AspNetRoles");

        //    modelBuilder.Entity<TUserRole>()
        //        .HasKeys((ur) => ur.UserId.ToString(),(ur) => ur.RoleId.ToString())
        //        .ToTable("AspNetUserRoles");

            modelBuilder.Entity<TUserLogin>()
                .HasKeys( (ul) => ul.UserId, 
                          (ul) => new { ul.LoginProvider, ul.ProviderKey})
                //Create and IndexTable to fast lookup based on LoginProvider and ProviderKey
                .WithIndex(ul => new { ul.LoginProvider, ul.ProviderKey })
                .ToTable("AspNetUserLogins");

            modelBuilder.Entity<TUserClaim>()
                //Quick lookup based on UserId
                .HasKeys(c=>c.UserId,c=>c.Id)
                .ToTable("AspNetUserClaims");


            base.OnModelCreating(modelBuilder);
        }

        public virtual ITableRepository<TUser> Users { get; set; }
        public virtual ITableRepository<TRole> Roles { get; set; }
        public virtual ITableRepository<TUserLogin> Logins { get; set; }
        public virtual ITableRepository<TUserClaim> Claims { get; set; }
    }
}
