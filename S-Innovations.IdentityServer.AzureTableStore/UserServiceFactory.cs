using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using SInnovations.Identity.AzureTableStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.AspNetIdentity;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.AzureTableStore
{

    public class UserServiceFactory
    {
        public static IUserService Factory<TUser>(IdentityTableContext<TUser> context)
            where TUser : IdentityUser, IUser<string>, new()
        {
           // var db = new IdentityDbContext<IdentityUser>("DefaultConnection");
         //   var store = new UserStore<IdentityUser>(db);
        //    var mgr = new UserManager<IdentityUser>(store);

         
            var store = new UserStore<TUser>(context);
            var mgr = new Microsoft.AspNet.Identity.UserManager<TUser>(store);
            var userSvc = new UserService<TUser, string>(mgr, context);
            return userSvc;

        }
    }
}
