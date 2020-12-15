using Finportal.Enum;
using Finportal.Models;
using Finportal.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Data
{
    public class ContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(PortalRole.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(PortalRole.HOH.ToString()));
            await roleManager.CreateAsync(new IdentityRole(PortalRole.Member.ToString()));
            //can seed bank accounts etc if i have a household.


        }
        public static async Task SeedDefaultUserAsync(UserManager<FPUser> userManager, IImageService imageService)
        {
            #region Admin
            //Seed for Admin User
            var defaultUser = new FPUser
            {
                UserName = "jontwin77@yahoo.com",
                Email = "jontwin77@yahoo.com",
                FirstName = "Jonathan",
                LastName = "Green",
                FileData = await imageService.AssignDefaultAvatarAsync("Shark.jpg"),
                FileName = "Shark.jpg",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, PortalRole.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("************** ERROR ****************");
                Debug.WriteLine("Error Seeding Default Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*********************************");
                throw;
            }
            #endregion
            #region HouseHold
            //Seed for Admin User
            defaultUser = new FPUser
            {
                UserName = "Vince@mailinator.com",
                Email = "Vince@mailinator.com",
                FirstName = "Vince",
                LastName = "Price",
                FileData = await imageService.AssignDefaultAvatarAsync("Shark.jpg"),
                FileName = "Shark.jpg",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, PortalRole.HOH.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("************** ERROR ****************");
                Debug.WriteLine("Error Seeding Default HOH User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*********************************");
                throw;
            }
            #endregion
            #region Member
            //Seed for Admin User
            defaultUser = new FPUser
            {
                UserName = "NDrake@mailinator.com",
                Email = "NDrake@mailinator.com",
                FirstName = "Nathan",
                LastName = "Drake",
                FileData = await imageService.AssignDefaultAvatarAsync("Shark.jpg"),
                FileName = "Shark.jpg",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, PortalRole.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("************** ERROR ****************");
                Debug.WriteLine("Error Seeding Default Member User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*********************************");
                throw;
            }
            #endregion

        }
    }
}
