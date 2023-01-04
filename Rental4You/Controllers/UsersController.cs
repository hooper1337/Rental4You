using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Rental4You.Controllers
{
    public class UsersController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = _context.Users;
            return View(await users.ToListAsync());
        }



        // GET: UsersController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = _userManager.IsInRoleAsync(user, "Admin");
            if (await result)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,firstName,lastName,bornDate,nif,available")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await makeUserAvailableUnavailable(id); 
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> makeUserAvailableUnavailable(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var currentUser = await _context.Users.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (currentUser == null)
            {
                return NotFound();
            }
            if (currentUser.available == true)
            {

                currentUser.available = false;
                var userTask = _userManager.FindByEmailAsync(currentUser.Email);
                userTask.Wait();
                var user = userTask.Result;
                var lockUserTask = _userManager.SetLockoutEnabledAsync(user, true);
                lockUserTask.Wait();
                var lockDateTask = _userManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);
                lockDateTask.Wait();
            }
            else
            {
                currentUser.available = true;
                var userTask = _userManager.FindByEmailAsync(currentUser.Email);
                userTask.Wait();
                var user = userTask.Result;
                var lockDisabledTask = _userManager.SetLockoutEnabledAsync(user, false);
                lockDisabledTask.Wait();
                var setLockoutEndDateTask = _userManager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromMinutes(1));
                setLockoutEndDateTask.Wait();
            }
            try
            {
                _context.Update(currentUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
