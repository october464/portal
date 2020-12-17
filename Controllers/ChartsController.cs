using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finportal.Data;
using Finportal.Models;
using Finportal.Models.Charts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Finportal.Controllers
{
    public class ChartsController : Controller
    {
        private readonly ApplicationsDbContext _context;
        private readonly UserManager<FPUser> _userManager;

        public ChartsController(ApplicationsDbContext context, UserManager<FPUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<JsonResult> BankPieChart()
        {
            List<ChartModel> result = new List<ChartModel>();

            var user = await _userManager.GetUserAsync(User);
            var accounts = _context.BankAccount.Where(b => b.HouseholdId == user.HouseholdId).ToList();
            foreach (var bank in accounts)
            {
                result.Add(new ChartModel
                {
                    Label = bank.Name,
                    Value = bank.CurrentBalance,
                });
            }
            return Json(result);
        }

        public JsonResult transactionChart()
        {
            List<TransactionChartModel> result = new List<TransactionChartModel>();

            var categories = _context.Category.ToList();
            foreach (var cat in categories)
            {
                result.Add(new TransactionChartModel
                {

                });
            }
            return Json(result);
        }
    }
}
