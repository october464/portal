
using ErrorFollower2020.Models.ViewModels;
using Finportal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Finportal.Extensions
{
    public class CustomUserManager : UserManager<FPUser>
    {
        public CustomUserManager(IUserStore<FPUser> store, IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<FPUser> passwordHasher, IEnumerable<IUserValidator<FPUser>> userValidators, 
            IEnumerable<IPasswordValidator<FPUser>> passwordValidators, ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<FPUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }
        public async Task<AvatarViewModel> GetUserAvatarAsync(ClaimsPrincipal principal)
        {
            var user = await GetUserAsync(principal);
            AvatarViewModel vm = new AvatarViewModel
            {
                ImagePath = user.FileName,
                ImageData = user.FileData
            };
            return vm;
        }
    }
}
