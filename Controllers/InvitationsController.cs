using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finportal.Data;
using Finportal.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Finportal.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Finportal.Controllers
{
    [Authorize]

    public class InvitationsController : Controller
    {
        private readonly ApplicationsDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<FPUser> _userManager;

        public InvitationsController(ApplicationsDbContext context, IEmailSender emailSender, UserManager<FPUser> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // GET: Invitations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Invitation.ToListAsync());
        }

        // GET: Invitations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invitation == null)
            {
                return NotFound();
            }

            return View(invitation);
        }

        // GET: Invitations/Create
        public IActionResult Create(int id)
        {
            var invitation = new Invitation() { HouseholdId = id, Expires = DateTime.Now };
            //ViewData["CategoryId"] = new SelectList(_context.Invitation.Where(c => c.RoleName = PortalRole), "Id", "Name");

            return View(invitation);
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HouseholdId,Expires,EmailTo,Subject,Body")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                invitation.HouseholdId = (int)user.HouseholdId;
                invitation.IsValid = true;
                invitation.Code = Guid.NewGuid();
                invitation.Created = DateTime.Now;
                _context.Invitation.Add(invitation);
                await _context.SaveChangesAsync();


                var callbackUrl = Url.Action("AcceptInvitation", "Invitations",
                    new { email = invitation.EmailTo, code = invitation.Code },
                    protocol: Request.Scheme);


                  
                var emailBody = $"{invitation.Body} <br/><p><h3>Please accept this invitation by <a href = '{HtmlEncoder.Default.Encode(callbackUrl)}'>Click here</a></h3></p>";
                 await _emailSender.SendEmailAsync(invitation.EmailTo, invitation.Subject, emailBody);
                //await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Households", new { id = invitation.HouseholdId});
            }
            return RedirectToAction("Details", "Households");


        }

        public async Task<IActionResult> AcceptInvitation(string email, string code)
        {
            //Step 1: Determine if the Invitation is good 
            //Sub-step 1: Find the invitation
            var invitation = _context.Invitation.FirstOrDefault(i => i.Code.ToString() == code);
            if (invitation == null)
            {
                return RedirectToAction("NotFound", new { email});
            }
            //Step 2: If found determine if it is still able to be used
            //  Sub-step  1: Check the IsValid flag
            if (!invitation.IsValid)
            {
                TempData["Message"] = $"Your invitation has been denied for the following reasons:";
                TempData["Message"] = $"<br/>It has been marked as invalid";

                return RedirectToAction("InvitationDenied", new { email });
            }
            //Step 3: Compare the expiration date against the current date
            if (DateTime.Now > invitation.Expires)
            {
                invitation.IsValid = false;
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Your invitation has been denied for the following reasons:";
                TempData["Message"] = $"<br/>The invitation expired on  {invitation.Expires:MMM dd, yyyy}";

                //Uh-oh ...this invitation is expired. It has to be marked as invalid
                return RedirectToAction("Expired", new { email });
            }
            //Step 4: I am to presume that the invitation is good 
            //Expectation:
            // sub-step 1: Mark the Invitation as Accepted
            //sub-step 2: Mark the Invitation as InValid
            //sub-step 3: Send the user to a custom registration page or maybe an action are controller?
            if (invitation.IsValid)
            {
                invitation.Accepted = true;
                invitation.IsValid = false;

            }
            await _context.SaveChangesAsync();

            //2 choices
            //Choice 1: Enhance the existing register page to handle both normal registration and registration as a result of a invite
            return RedirectToPage("/Account/Register", new { area = "Identity", email = invitation.EmailTo, code = invitation.Code});

            //return RedirectToAction("SpecialRegistration", new { email});

        }
        

        public IActionResult InvitationDenied()
        {
            return View();
        }

        // GET: Invitations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitation.FindAsync(id);
            if (invitation == null)
            {
                return NotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HouseholdId,Created,Expires,Accepted,EmailTo,Subject,Body,RoleName,Code")] Invitation invitation)
        {
            if (id != invitation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invitation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvitationExists(invitation.Id))
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
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invitation == null)
            {
                return NotFound();
            }

            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invitation = await _context.Invitation.FindAsync(id);
            _context.Invitation.Remove(invitation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvitationExists(int id)
        {
            return _context.Invitation.Any(e => e.Id == id);
        }
    }
}
//public async Task<IActionResult> NotFound(string email)
//{
//    ViewData["Email"] = email;
//    return View();
//}
//public async Task<IActionResult> IsValid(string email)
//{
//    ViewData["Email"] = email;
//    return View();
//}

//public async Task<IActionResult> Expired(string email)
//{
//    ViewData["Email"] = email;
//    return View();
//}