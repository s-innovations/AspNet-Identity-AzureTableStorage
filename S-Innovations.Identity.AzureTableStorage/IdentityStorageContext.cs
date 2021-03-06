﻿using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage;
using SInnovations.Azure.TableStorageRepository;
using SInnovations.Azure.TableStorageRepository.TableRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Identity.AzureTableStorage
{


    public class IdentityTableContext<TUser, TKey, TUserRole, TUserLogin, TUserClaim> : TableStorageContext
        where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TUserRole : IdentityRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TKey : IEquatable<TKey>
    {

        public IdentityTableContext(CloudStorageAccount account)
            : base(account)
        {

        }

        protected override void OnModelCreating(TableStorageModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TUser>()
                .HasKeys(u => u.Id, u => "")
                .WithIndex(u => u.UserName)
                .WithIndex(u => u.Email)
                .WithPropertyOf(u=>u.Claims)
            .ToTable("AspNetUsers");

            modelBuilder.Entity<TUserRole>()
                .HasKeys(r => r.UserId, r => r.Name)
                .ToTable("AspNetRoles");

            modelBuilder.Entity<TUserLogin>()
                .UseBase64EncodingFor(ul => ul.ProviderKey)
                .HasKeys((ul) => ul.UserId,
                         (ul) => new { ul.LoginProvider, ul.ProviderKey })
                //Create and IndexTable to fast lookup based on LoginProvider and ProviderKey
                .WithIndex(ul => new { ul.LoginProvider, ul.ProviderKey })                
                .ToTable("AspNetUserLogins");

            modelBuilder.Entity<TUserClaim>()
                //Quick lookup based on UserId
                .HasKeys(c => c.UserId, c => c.ClaimType)
                .ToTable("AspNetUserClaims");

            base.OnModelCreating(modelBuilder);
        }

        public virtual ITableRepository<TUser> Users { get; set; }
        public virtual ITableRepository<TUserRole> Roles { get; set; }
        public virtual ITableRepository<TUserLogin> Logins { get; set; }
        public virtual ITableRepository<TUserClaim> Claims { get; set; }
    }
    public class IdentityRole : IdentityRole<string>
    {
        // Methods
        public IdentityRole()
        {
            base.Id = Guid.NewGuid().ToString();
        }

        public IdentityRole(string roleName)
            : this()
        {
            base.Name = roleName;
        }
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

        public IdentityUser(string userName)
            : this()
        {
            this.UserName = userName;
        }
    }


    public class IdentityTableContext<TUser> : IdentityTableContext<TUser, string, IdentityRole, IdentityUserLogin, IdentityUserClaim>
        where TUser : IdentityUser
    {
        public IdentityTableContext(CloudStorageAccount account)
            : base(account)
        {

        }
        protected override void OnModelCreating(TableStorageModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TUser>()
     //         .WithCollectionOf<IdentityUserClaim>(u => u.Claims, (source, user) => (from claim in source where claim.UserId == user.Id select claim))
              .WithCollectionOf<IdentityUserLogin>(u => u.Logins, (source, user) => (from login in source where login.UserId == user.Id select login))
              .WithCollectionOf<IdentityRole>(u => u.Roles, (source, user) => (from role in source where role.UserId == user.Id select role));

            base.OnModelCreating(modelBuilder);


        }
    }
}
