using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finportal.Data;
using Finportal.Models;
using Microsoft.AspNetCore.Authorization;
using Finportal.Enum;
using Microsoft.AspNetCore.Identity;

namespace Finportal.Controllers
{
   
    //[Authorize]
    public class CategoryItemsController : Controller
    {
        private readonly ApplicationsDbContext _context;
        private readonly UserManager<FPUser> _userManager;

        public CategoryItemsController(ApplicationsDbContext context, UserManager<FPUser> userManager )
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CategoryItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.CategoryItem.ToListAsync());
        }

        // GET: CategoryItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryItem = await _context.CategoryItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryItem == null)
            {
                return NotFound();
            }

            return View(categoryItem);
        }

        // GET: CategoryItems/Create
        //[Authorize(Roles = "HOH, Memeber,Admin")]
        public async Task<IActionResult> Create(int? id)
        {
            var householdId = (await _userManager.GetUserAsync(User)).HouseholdId;
            var categoryItem = new CategoryItem() { CategoryId = (int)id };
            ViewData["CategoryId"] = new SelectList(_context.Category.Where(c => c.HouseholdId == householdId), "Id", "Name");
            return View(categoryItem);
        }

        // POST: CategoryItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CategoryId,Description,TargetAmount")] CategoryItem categoryItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoryItem);
                await _context.SaveChangesAsync();
                var category = await _context.Category.Include(c =>c.Household).FirstOrDefaultAsync(c => c.Id == categoryItem.CategoryId );
                return RedirectToAction("Details","Households", new { id = category.HouseholdId });
            }
            return View(categoryItem);
        }

        // GET: CategoryItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryItem = await _context.CategoryItem.FindAsync(id);
            if (categoryItem == null)
            {
                return NotFound();
            }
            return View(categoryItem);
        }

        // POST: CategoryItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Name,Description,TargetAmount,ActualAmount,Code")] CategoryItem categoryItem)
        {
            if (id != categoryItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoryItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryItemExists(categoryItem.Id))
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
            return View(categoryItem);
        }

        // GET: CategoryItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryItem = await _context.CategoryItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryItem == null)
            {
                return NotFound();
            }

            return View(categoryItem);
        }

        // POST: CategoryItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryItem = await _context.CategoryItem.FindAsync(id);
            _context.CategoryItem.Remove(categoryItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryItemExists(int id)
        {
            return _context.CategoryItem.Any(e => e.Id == id);
        }
    }
}
