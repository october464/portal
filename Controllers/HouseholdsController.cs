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
using Finportal.Enum;
using Microsoft.AspNetCore.Authorization;
using Finportal.Models.ViewModel;

namespace Finportal.Controllers
{
    [Authorize]

    public class HouseholdsController : Controller
    {
        private readonly ApplicationsDbContext _context;
        private readonly UserManager<FPUser> _userManager;
        private readonly SignInManager<FPUser> _signInManager;


        public HouseholdsController(ApplicationsDbContext context, UserManager<FPUser> userManager, SignInManager<FPUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Households
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            //var user = await _userManager.FindByIdAsync(userId);

            
            return View(await _context.Household.ToListAsync());


        }
        public async Task<IActionResult> Dashboard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var household = await _context.Household
                .Include(h => h.Members)
                .ThenInclude(u => u.Transactions)
                .Include(h => h.Members)
                .Include(u => u.Categories)
                .ThenInclude(h => h.CategoryItems)
                .Include(h => h.Members)
                .ThenInclude(u => u.BankAccounts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (household==null)
            {
                return NotFound();
            }
            return View(household);
        }

        //Leave
        [Authorize(Roles= "HOH, Member")]
        public async Task<IActionResult> Leave()

        {
           
            //Determine who I am here....I dont need any additional info
            //Step 1: Get User Record
            var user = await _userManager.GetUserAsync(User);
            var memberCount = _context.Users.Where(u => u.HouseholdId == user.HouseholdId).Count();

            //Determine is I am actually able to leave or do I have to warn the user
            if (User.IsInRole(nameof(PortalRole.HOH)) && memberCount > 1)
            {
                //Load up a nasty gram to display to the HOH
                TempData["Message"] = "You cannot leave the Household untill all the Members have left";
                return RedirectToAction("Dashboard");
            }

            //Step 2: Remove the HouseHoldId
            var householdIdMemento = user.HouseholdId;
                user.HouseholdId = null;
                await _context.SaveChangesAsync();

                //Step 3: get the user's role
                var myRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                //Step 4: Remove them from that role
                await _userManager.RemoveFromRoleAsync(user, myRole);

                //Step 5: Refresh user login
                await _signInManager.RefreshSignInAsync(user);

                //Step 6: If there is nobody left in the Household mark it as deleted or delete it 
                if (_context.Users.Where(u => u.HouseholdId == householdIdMemento).Count() == 0)
                {
                //Delete the house
                 var household = await (_context.Household.FindAsync(householdIdMemento));
                await _context.SaveChangesAsync();

                //What happens to all the children?


                }

                //Step 7: Redirect to the lobby
                return RedirectToAction("Lobby, Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> hohLeave(string newHOH)
        {
            var user = await _userManager.GetUserAsync(User);
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            await _userManager.RemoveFromRoleAsync(user, role);
            var newHead = await _context.Users.FindAsync(newHOH);
            await _userManager.RemoveFromRoleAsync(newHead, nameof(PortalRole.Member));

            await _userManager.AddToRoleAsync(newHead, nameof(PortalRole.HOH));
            user.HouseholdId = null;
            await _context.SaveChangesAsync();

            return RedirectToAction("Lobby", "Home");
        }
        [Authorize(Roles = "HOH, Member")]
        // GET: Households/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            
            var household = await _context.Household
               
                .Include(h => h.Members)
                .Include(h => h.Members)
                .Include(u => u.Categories)
                .ThenInclude(h => h.CategoryItems)
                .Include(h => h.Members)
                .ThenInclude(u => u.BankAccounts)
                .ThenInclude(u => u.Transactions)
                .Include(h => h.Members)
                .ThenInclude(h => h.Household)
                .FirstOrDefaultAsync(m => m.Id == id);
          
            var classViewM = new ClassViewModel()
            {
               Accounts = household.BankAccounts.ToList(),
               Categories = household.Categories.ToList(),
               CategoryItems = household.Categories.SelectMany(c => c.CategoryItems).ToList(),
               Established = household.Established,
               Greeting = household.Greeting,
               Id = household.Id,
               Invitations = household.Invitations.ToList(),
               Name = household.Name,
               Notifications = household.Notifications.ToList(),
               Members = household.Members.ToList(),
               Transactions = household.BankAccounts.SelectMany(t => t.Transactions).ToList(),
                
               

               //Equations to help with calculations later on
               HouseholdBalance =household.BankAccounts.Sum(cb => cb.CurrentBalance)
            };
            ViewData["RoleName"] = new SelectList(_context.Roles.Where(n => n.Name != PortalRole.Admin.ToString()), "Name", "Role");
            ViewData["MemberIds"] = new SelectList(classViewM.Members, "Id", "FullName");

            //household.Users = (List<FPUser>) _context.Users.Where(h => h.HouseholdId == id).ToListAsync();
            if (household == null)
            {
                return NotFound();
            }

            //ViewBag["MemberId"] = new SelectList(dashboardVm.)
            return View(household);
        }

        // GET: Households/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Greeting")] Household household)
        {
            if (ModelState.IsValid)
            {
                household.Established = DateTime.Now;

                _context.Add(household);
                await _context.SaveChangesAsync();


                var currentUser = await _userManager.GetUserAsync(User);
                currentUser.HouseholdId = household.Id;

                await _userManager.AddToRoleAsync(currentUser, PortalRole.HOH.ToString());
                await _context.SaveChangesAsync();

                await _signInManager.RefreshSignInAsync(currentUser);
               

                return RedirectToAction("Details", "Households", new { id = household.Id });
            }
            return View(household);
        }

        // GET: Households/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var household = await _context.Household.FindAsync(id);
            if (household == null)
            {
                return NotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Greeting,Established")] Household household)
        {
            if (id != household.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(household);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HouseholdExists(household.Id))
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
            return View(household);
        }

        // GET: Households/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var household = await _context.Household
                .FirstOrDefaultAsync(m => m.Id == id);
            if (household == null)
            {
                return NotFound();
            }

            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var household = await _context.Household.FindAsync(id);
            _context.Household.Remove(household);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HouseholdExists(int id)
        {
            return _context.Household.Any(e => e.Id == id);
        }
    }
}
