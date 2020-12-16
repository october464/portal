using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finportal.Data;
using Finportal.Models.Charts;
using Microsoft.AspNetCore.Mvc;

namespace Finportal.Controllers
{
    public class ChartsController : Controller
    {
        private readonly ApplicationsDbContext _context;
        public ChartsController(ApplicationsDbContext context)
        {
            _context = context;
        }

        public JsonResult BankPieChart()
        {
            List<ChartModel> result = new List<ChartModel>();

            var account = _context.BankAccount.ToList();
            foreach (var bank in account)
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
