using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finportal.Data;
using Finportal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Finportal.Controllers
{
    [Authorize]

    public class BankAccountsController : Controller
    {
        private readonly ApplicationsDbContext _context;
        private readonly UserManager<FPUser> _userManager;

        public BankAccountsController(ApplicationsDbContext context, UserManager<FPUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BankAccounts
        public async Task<IActionResult> Index()
        {

            return View(await _context.BankAccount.ToListAsync());
        }

        // GET: BankAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        //[Authorize(Roles= "HOH, Member")]
        public async Task<IActionResult> Create()
        {
            var account = new BankAccount { HouseholdId = (int)(await _userManager.GetUserAsync(User)).HouseholdId };
            return View(account);
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HouseholdId,Name,Type,StartingBalance")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                bankAccount.FPUserId = _userManager.GetUserId(User);
                bankAccount.CurrentBalance = bankAccount.StartingBalance;
                _context.Add(bankAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details","Households", new { id = bankAccount.HouseholdId });
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccount.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HouseholdId,FPUserId,Name,Transaction,Type,StartingBalance,CurrentBalance")] BankAccount bankAccount)
        {
            if (id != bankAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bankAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankAccountExists(bankAccount.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bankAccount = await _context.BankAccount.FindAsync(id);
            _context.BankAccount.Remove(bankAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BankAccountExists(int id)
        {
            return _context.BankAccount.Any(e => e.Id == id);
        }
    }
}
