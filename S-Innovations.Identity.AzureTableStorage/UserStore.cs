using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using Microsoft.WindowsAzure.Storage.Table;

namespace SInnovations.Identity.AzureTableStorage
{

    public class UserStore<TUser> : UserStore<TUser, string, IdentityRole, IdentityUserLogin, IdentityUserClaim>, IDisposable where TUser : IdentityUser
    {
        // Methods


        public UserStore(IdentityTableContext<TUser> context)
            : base(context)
        {
        }
    }

    public class UserStore<TUser, TKey,TRole, TUserLogin, TUserClaim> : 
        IUserLoginStore<TUser, TKey>, 
        IUserClaimStore<TUser, TKey>, 
        IUserRoleStore<TUser, TKey>, 
        IUserPasswordStore<TUser, TKey>, 
        IUserSecurityStampStore<TUser, TKey>, 
        IQueryableUserStore<TUser, TKey>, 
        IUserEmailStore<TUser, TKey>, 
        IUserPhoneNumberStore<TUser, TKey>, 
        IUserTwoFactorStore<TUser, TKey>, 
        IUserStore<TUser, TKey>, 
        IDisposable
        where TUser : IdentityUser<TKey, TUserLogin, TRole, TUserClaim>
        where TRole : IdentityRole<TKey>,new()
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
     //   where TUserRole : IdentityUserRole<TKey>, new()
        where TUserClaim : IdentityUserClaim<TKey>, new()
    {
        private IdentityTableContext<TUser, TKey, TRole, TUserLogin, TUserClaim> _context;
        private bool _disposed;
        public UserStore(IdentityTableContext<TUser, TKey,TRole, TUserLogin, TUserClaim> tableContext)
        {
            _context = tableContext;
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            this.ThrowIfDisposed();

            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("loginInfo");
           
            TUserLogin item = Activator.CreateInstance<TUserLogin>();
            item.UserId = user.Id;
            item.ProviderKey = login.ProviderKey;
            item.LoginProvider = login.LoginProvider;
            user.Logins.Add(item);
            return Task.FromResult<int>(0);
        }

        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
         
            this.ThrowIfDisposed();
            
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            TUserLogin userLogin = await _context.Logins.FindByIndexAsync(login.LoginProvider, login.ProviderKey);
          
            if (userLogin != null)
            {
                return await FindByIdAsync(userLogin.UserId);
            }

            return default(TUser);

        }
        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<IList<UserLoginInfo>>((from l in user.Logins select new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList<UserLoginInfo>());
        
                //var query = from ent in _context.Logins.Source
                //             where ent.PartitionKey == user.Id.ToString()

        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            string provider = login.LoginProvider;
            string key = login.ProviderKey;
            TUserLogin item = user.Logins.SingleOrDefault<TUserLogin>(l => (l.LoginProvider == provider) && (l.ProviderKey == key));
            if (item != null)
            {
                user.Logins.Remove(item);
            }
            return Task.FromResult<int>(0);

        }

        public async Task CreateAsync(TUser user)
        {
           
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _context.Users.Delete(user);

            await _context.SaveChangesAsync();
        }

        public Task<TUser> FindByIdAsync(TKey userId)
        {
            ThrowIfDisposed();

            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }
            return _context.Users.FindByKeysAsync(userId.ToString(), "");
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();

            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            return _context.Users.FindByIndexAsync(userName);
        }

        public async Task UpdateAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            //if ((this.DisposeContext && disposing) && (this._context != null))
            //{
            //    this._context.Dispose();
            //}
            this._disposed = true;
            this._context = null;
         //   this._userStore = null;
        }
        
        public Task AddClaimAsync(TUser user, System.Security.Claims.Claim claim)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            TUserClaim item = Activator.CreateInstance<TUserClaim>();
            item.UserId = user.Id;
            item.ClaimType = claim.Type;
            item.ClaimValue = claim.Value;
            user.Claims.Add(item);
            return Task.FromResult<int>(0);

        }

        public Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<IList<Claim>>((from c in user.Claims select new Claim(c.ClaimType, c.ClaimValue)).ToList<Claim>());

        }

        public Task RemoveClaimAsync(TUser user, System.Security.Claims.Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            foreach (TUserClaim local in (from uc in user.Claims
                                          where (uc.ClaimValue == claim.Value) && (uc.ClaimType == claim.Type)
                                          select uc).ToList<TUserClaim>())
            {
                user.Claims.Remove(local);
            }

            return Task.FromResult<int>(0);

        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("roleName cannot be null");
            }

            var local = (from ent in this._context.Roles
                         where ent.RowKey == roleName
                         select ent).Take(1).FirstOrDefault();
                      
            if (local ==null)
            {
                throw new InvalidOperationException("RoleName do not exist");
            }
           
            TRole local3 = Activator.CreateInstance<TRole>();
            local3.Id = user.Id;
            local3.Name = roleName;
            user.Roles.Add(local3);
           
            return Task.FromResult<int>(0);

        }

        public async Task<IList<string>> GetRolesAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
             List<string> claims = new List<string>();
            var query = (from ent in this._context.Roles
                        where ent.PartitionKey == user.Id.ToString()
                        select ent).AsTableQuery();

            TableQuerySegment<TRole> querySegment = null;
            while (querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await query.ExecuteSegmentedAsync(querySegment != null ? querySegment.ContinuationToken : null);
                claims.AddRange(querySegment.Results.Select(x => x.Name));
            }

            return claims;


        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            this.ThrowIfDisposed();

            if (user == null) throw new ArgumentNullException("user");
            if (String.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException("role");
            return (await _context.Roles.FindByKeysAsync(user.Id.ToString(),roleName) != null);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<string>(user.PasswordHash);

        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult<bool>(user.PasswordHash != null);

        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult<int>(0);


        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<string>(user.SecurityStamp);

        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult<int>(0);

        }

        public IQueryable<TUser> Users
        {
            get { return _context.Users; }
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();

            if (email == null)
            {
                throw new ArgumentNullException("email");
            }

            return _context.Users.FindByIndexAsync(email);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<string>(user.Email);

        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.EmailConfirmed);

        }

        public Task SetEmailAsync(TUser user, string email)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Email = email;
            return Task.FromResult<int>(0);

        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = confirmed;
            return Task.FromResult<int>(0);

        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<string>(user.PhoneNumber);

        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.PhoneNumberConfirmed);

        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumber = phoneNumber;
            return Task.FromResult<int>(0);

        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult<int>(0);

        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.TwoFactorEnabled);

        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.TwoFactorEnabled = enabled;
            return Task.FromResult<int>(0);

        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(base.GetType().Name);
            }
        }


    }
}
