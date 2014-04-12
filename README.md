[![GoogleAnalyticsTracker Nightly Build Status](https://www.myget.org/BuildSource/Badge/s-innovations?identifier=7849c88e-36e8-4151-9ace-eddbf7e74363)](https://www.myget.org/gallery/googleanalyticstracker)


AspNet-Identity-AzureTableStorage
=================================

A implementation for AspNet Identity that uses Azure Table Storage. Its a test case for my Table Storage Repository Library


The project is a test project for a library I wrote. Its 4 days of work and properly have some bugs. But the idea was to create a abstraction layer for table storage that would make it easy to use table storage and this project was then the test case to find out if it worked or not.

´´´
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
                .ToTable("AspNetRoles");

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
    
´´´
