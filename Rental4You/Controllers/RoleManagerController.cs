using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Rental4You.Controllers
{
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleManagerController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _roleManager.Roles.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
                await this._roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string role)
        {
            if (role != null)
                await _roleManager.DeleteAsync(_roleManager.Roles.Where(r => r.Id == role).FirstOrDefault());
            return RedirectToAction("Index");
        }
    }
}
