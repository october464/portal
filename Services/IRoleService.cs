using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models
{
    public interface IRoleService
    {
        public Task<bool> AddUserToRole(FPUser user, string roleName);

        public Task<bool> IsUserInRole(FPUser user, string roleName);


        public Task<IEnumerable<string>> ListUserRoles(FPUser user);

        public Task<bool> RemoveUserFromRole(FPUser user, string roleName);

        public Task<ICollection<FPUser>> UserInRole(string roleName);

        public Task<ICollection<FPUser>> UserNotInRole(IdentityRole role);
    }
}
