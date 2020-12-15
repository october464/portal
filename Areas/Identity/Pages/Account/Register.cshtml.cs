using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Finportal.Data;
using Finportal.Enum;
using Finportal.Extensions;
using Finportal.Models;
using Finportal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Finportal.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<FPUser> _signInManager;
        private readonly UserManager<FPUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IImageService _imageService;
        private readonly ApplicationsDbContext _dbContext;

        public RegisterModel(
            UserManager<FPUser> userManager,
            SignInManager<FPUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IImageService imageService,
            ApplicationsDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _imageService = imageService;
            _dbContext = dbContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(25, ErrorMessage ="The {0} must be at least {2} and at max {1} characters long.", MinimumLength =2)]
            [Display(Name = "FirstName")]
            public string FirstName { get; set; }


            [Required]
            [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [Display(Name = "LastName")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Avatar")]
            [NotMapped]
          
            [MaxFileSize(2 * 1024 *1024)]
            [AllowedExtenstions(new string[] { ".jpg",".jpeg", ".png", ".gif"})]
            public IFormFile FormFile { get; set; }

            public string Code { get; set; }
        }

        public async Task OnGetAsync(string email, string code,string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Email = email;
            Code = code;
           
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //This is what actually creating our user
                var user = new FPUser
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    UserName = Input.Email,
                    Email = Input.Email,
                    FileName = "Shark.jpg",
                    FileData = await _imageService.AssignDefaultAvatarAsync("Shark.jpg")
                };
                if (!string.IsNullOrEmpty(Input.Code))
                {
                  
                    user.HouseholdId = _dbContext.Invitation.FirstOrDefault(i => i.Code.ToString() == Input.Code).HouseholdId;
                    user.EmailConfirmed = true;
                }
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (!string.IsNullOrEmpty(Input.Code))
                {
                    await _userManager.AddToRoleAsync(user, PortalRole.Member.ToString());
                    await _signInManager.RefreshSignInAsync(user);
                    return RedirectToAction("Details", "Households", new { id = user.HouseholdId});
                }
                if (Input.FormFile != null)
                {
                    user.FileName = Input.FormFile.FileName;
                    user.FileData = await _imageService.ConvertFileToByteArrayAsync(Input.FormFile);
                }
                //var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                //var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
