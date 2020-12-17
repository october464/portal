using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Finportal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Finportal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<FPUser> _userManager;

        //private readonly SignInManager

        public HomeController(ILogger<HomeController> logger, UserManager<FPUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.HouseholdId != null)
            {
                return RedirectToAction("Details", "Households", new { id = user.HouseholdId });
            }
            else
            {

                return RedirectToAction("Lobby");
            }
        }
       
        public async Task<IActionResult> Lobby()
        {

            return View();


        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
