using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;

namespace Rental4You.Controllers
{
    public class UserRolesManagerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRolesManagerController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new List<UserRolesViewModel>();

            foreach (var u in users)
            {
                var user = new UserRolesViewModel();
                user.UserId = u.Id;
                user.Username = u.UserName;
                user.Roles = await GetUserRoles(u);
                user.firstName = u.firstName;
                user.lastName = u.lastName;
                userRoles.Add(user);
            }

            return View(userRoles);
        }

        [Authorize(Roles = "Admin")]
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserId = userId;
            ViewBag.UserName = user.UserName;
            var model = new List<ManageUserRolesViewModel>();

            foreach (var role in _roleManager.Roles)
            {
                var userRolesManager = new ManageUserRolesViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                userRolesManager.Selected = await _userManager.IsInRoleAsync(user, role.Name);
                model.Add(userRolesManager);
            }
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(x => x.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add");
                return View(model);
            }
            return RedirectToAction("Index");
        }
    }
}
