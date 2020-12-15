using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models
{
    public class RoleService : IRoleService
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<FPUser> _userManager;
        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<FPUser> userManager)//constructor
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<bool> AddUserToRole(FPUser user, string roleName)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }
        public async Task<bool> IsUserInRole(FPUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);

        }


        public async Task<IEnumerable<string>> ListUserRoles(FPUser user)
        {
            return await _userManager.GetRolesAsync(user);

        }

        public async Task<bool> RemoveUserFromRole(FPUser user, string roleName)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<ICollection<FPUser>> UserInRole(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            return users;
        }

        public async Task<ICollection<FPUser>> UserNotInRole(IdentityRole role)
        {
            var roleId = await _roleManager.GetRoleIdAsync(role);
            return await _userManager.Users.Where(u => IsUserInRole(u, role.Name).Result == false).ToListAsync();
        }
    }
}
