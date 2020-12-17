
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finportal.Data;
using Finportal.Models;
using Microsoft.AspNetCore.Identity;
using Finportal.Services;
using Microsoft.AspNetCore.Authorization;

namespace Finportal.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationsDbContext _context;
        private readonly UserManager<FPUser> _userManager;
        private readonly INotificationService _notificationService;

        public TransactionsController(ApplicationsDbContext context, UserManager<FPUser> userManager, INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transaction.Include(t => t.BankAccount).Include(t => t.CategoryItem).Include(t => t.FPUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.BankAccount)
                .Include(t => t.CategoryItem)
                .Include(t => t.FPUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public async Task<IActionResult> Create()
        {
            var householdId = (await _userManager.GetUserAsync(User)).HouseholdId;//null reference
            var myBankAccounts = _context.BankAccount.Where(b => b.HouseholdId == householdId);
            ViewData["BankAccountId"] = new SelectList(myBankAccounts, "Id", "Name");
            ViewData["CategoryItemId"] = new SelectList(_context.CategoryItem, "Id", "Name");
            ViewData["FPUserId"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryItemId,BankAccountId,Created,Type,Memo,Amount")] Transaction transaction)
        {


            if (ModelState.IsValid)
            {
                transaction.FPUserId = _userManager.GetUserId(User);
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                //1:Decrease the Bank Current Balance
                var account = await _context.BankAccount.FindAsync(transaction.BankAccountId);
                var oldBalance = account.CurrentBalance;

                if (transaction.Type == Enum.TransactionType.Deposit)
                {
                    account.CurrentBalance += transaction.Amount;
                }
                else
                {
                    //Withdrawl
                    account.CurrentBalance -= transaction.Amount;
                    //2:Increase the Actual Amount of the associated Category Item
                    var categoryItem = await _context.CategoryItem.FindAsync(transaction.CategoryItemId);
                    categoryItem.ActualAmount += transaction.Amount;
                }
                await _context.SaveChangesAsync();
                //3:Look for any needed notifications
                if (account.CurrentBalance < 0)
                {
                    TempData["OverDraft"] = "you overdrft your account. FOO";
                }
                await _notificationService.NotifyOverdraftAsync(transaction, account, oldBalance);
                return RedirectToAction("Details", "Households", new { id = (await _userManager.GetUserAsync(User)).HouseholdId });

            }
            ViewData["BankAccountId"] = new SelectList(_context.BankAccount, "Id", "Name", transaction.BankAccountId);
            ViewData["CategoryItemId"] = new SelectList(_context.CategoryItem, "Id", "Name", transaction.CategoryItemId);
            ViewData["FPUserId"] = new SelectList(_context.Users, "Id", "FullName", transaction.FPUserId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["BankAccountId"] = new SelectList(_context.BankAccount, "Id", "Name", transaction.BankAccountId);
            ViewData["CategoryItemId"] = new SelectList(_context.CategoryItem, "Id", "Name", transaction.CategoryItemId);
            ViewData["FPUserId"] = new SelectList(_context.Users, "Id", "FullName", transaction.FPUserId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryItemId,BankAccountId,FPUserId,Created,Type,Memo,Amount,IsDeleted")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldTransaction = _context.Transaction.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);

                    _context.Update(transaction);
                  
                    var bankAccount = _context.BankAccount.Find(transaction.BankAccountId);
                    if (oldTransaction.Amount > transaction.Amount)
                    {
                        var result = oldTransaction.Amount - transaction.Amount;
                        bankAccount.CurrentBalance += result;
                        _context.Update(bankAccount);
                    }
                    else
                    {
                        var result = oldTransaction.Amount - transaction.Amount;

                        bankAccount.CurrentBalance += result;
                        _context.Update(bankAccount);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
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
            ViewData["BankAccountId"] = new SelectList(_context.BankAccount, "Id", "Name", transaction.BankAccountId);
            ViewData["CategoryItemId"] = new SelectList(_context.CategoryItem, "Id", "Id", transaction.CategoryItemId);
            ViewData["FPUserId"] = new SelectList(_context.Users, "Id", "Id", transaction.FPUserId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.BankAccount)
                .Include(t => t.CategoryItem)
                .Include(t => t.FPUser)
                .FirstOrDefaultAsync(m => m.Id == id);


            var account = await _context.BankAccount.Include(t => t.Household).FirstOrDefaultAsync(t => t.Id == transaction.BankAccountId);

            account.CurrentBalance += transaction.Amount;
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transaction.FindAsync(id);

            var account = await _context.BankAccount.Include(t => t.Household).FirstOrDefaultAsync(t => t.Id == transaction.BankAccountId);

            account.CurrentBalance += transaction.Amount;

            _context.Transaction.Remove(transaction);

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Households", new { id = account.HouseholdId });
        }

        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.Id == id);
        }
    }
}
