
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;

namespace Rental4You.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IUserStore<ApplicationUser> _userStore;
        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
        }

        // GET: Companies
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
              return View(await _context.companies.Include("vehicles").ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(string? textToSearch, Boolean? available)
        {
            SearchCompanyViewModel searchCompany = new SearchCompanyViewModel();
            if (string.IsNullOrWhiteSpace(textToSearch) && available == null)
            {
                searchCompany.companyList = await _context.companies.Include("vehicles").ToListAsync();
            }
            if (available != null && textToSearch != null)
            {
                searchCompany.companyList = await _context.companies.Include("vehicles").Where(c => c.available == available && c.name.Contains(textToSearch)).ToListAsync();
            }

            if (textToSearch != null && available == null)
            {
                searchCompany.companyList = await _context.companies.Include("vehicles").Where(c => c.name == textToSearch).ToListAsync();
            }

            if (available != null && textToSearch == null)
            {
                searchCompany.companyList = await _context.companies.Include("vehicles").Where(c => c.available == available).ToListAsync();
            }
            searchCompany.numberOfResults = await _context.companies.CountAsync();
            return View(searchCompany);
        }


        // GET: Companies/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.companies == null)
            {
                return NotFound();
            }

            var company = await _context.companies.Include("vehicles")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ManagerDashboard()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var manager = _context.managers.Where(x => x.applicationUser.Id == applicationUserId).First();

            DateTime currentDate = DateTime.Now;
            DateTime last7Days = currentDate.AddDays(-7);
            DateTime last30Days = currentDate.AddDays(-30);

            var reservationsLast7Days = await _context.reservations.Include("vehicle")
                                                              .Where(r => r.DateTimeOfRequest > last7Days 
                                                                     && r.DateTimeOfRequest < currentDate
                                                                     && r.vehicle.CompanyId == manager.CompanyId
                                                                     )
                                                              .ToListAsync();

            var reservationsLast30Days = await _context.reservations.Include("vehicle")
                                                              .Where(r => r.DateTimeOfRequest > last30Days
                                                                && r.DateTimeOfRequest < currentDate
                                                                && r.vehicle.CompanyId == manager.CompanyId
                                                                )
                                                              .ToListAsync();

           
            var invoice7Days = 0;
            var invoice30Days = 0;

            var numberOfReservs = reservationsLast30Days.Count;
            var diaryReservsEachDay = Math.Round((decimal)numberOfReservs/30, 2);

            foreach (var reserv in reservationsLast7Days)
            {
                invoice7Days = invoice7Days + Convert.ToInt32(reserv.Price);
            }
            foreach (var reserv in reservationsLast30Days)
            {
                invoice30Days = invoice30Days + Convert.ToInt32(reserv.Price);
            }

            ViewData["Invoice7Days"] = invoice7Days;
            ViewData["Invoice30Days"] = invoice30Days;
            ViewData["AverageReserv"] = diaryReservsEachDay;

            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,name,classification,available")] Company company)
        {
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                var email = "mainmanager"  + company.name.ToLower() + "@gmail.com";
                await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
                user.firstName = "Manager";
                user.lastName = company.name;
                user.nif = 0;
                user.bornDate = DateTime.Today;
                user.EmailConfirmed = true;
                user.available = true;

                var result = await _userManager.CreateAsync(user, "Jogodohugo2001!");
                if(result.Succeeded)
                {
                    _context.Add(company);
                    await _context.SaveChangesAsync();
                    var manager = new Manager
                    {
                        CompanyId = company.Id,
                        company = company,
                        applicationUser = user
                    };
                    _context.Update(manager);
                    await _context.SaveChangesAsync();
                    await _userManager.AddToRoleAsync(user, "Manager");
                    return RedirectToAction(nameof(Index));
                }
               
            }
            return View(company);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ListCompanyEmployees()
        {
            var applicationUserId = _userManager.GetUserId(User);
            var manager = _context.managers.Where(m => m.applicationUser.Id == applicationUserId).FirstOrDefault();
            var employees = _context.employees.Include("applicationUser").Include("company").Where(e => e.CompanyId == manager.CompanyId);
            return View(await employees.ToListAsync());
        }

        // GET: Companies/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.companies == null)
            {
                return NotFound();
            }

            var company = await _context.companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> makeEmployeeAvailableUnavailable(int? id)
        {
            if (id == null || _context.employees == null)
            {
                return NotFound();
            }

            var employee = await _context.employees.Include("applicationUser").Where(e => e.Id == id).FirstOrDefaultAsync();
            if (employee == null)
            {
                return NotFound();
            }
            if (employee.applicationUser.available == true) {

                employee.applicationUser.available = false;
                var userTask = _userManager.FindByEmailAsync(employee.applicationUser.Email);
                userTask.Wait();
                var user = userTask.Result;
                var lockUserTask = _userManager.SetLockoutEnabledAsync(user, true);
                lockUserTask.Wait();
                var lockDateTask = _userManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);
                lockDateTask.Wait();
            }
            else 
            {
                employee.applicationUser.available = true;
                var userTask = _userManager.FindByEmailAsync(employee.applicationUser.Email);
                userTask.Wait();
                var user = userTask.Result;
                var lockDisabledTask = _userManager.SetLockoutEnabledAsync(user, false);
                lockDisabledTask.Wait();
                var setLockoutEndDateTask = _userManager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromMinutes(1));
                setLockoutEndDateTask.Wait();
            }
            try
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(employee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(ListCompanyEmployees));
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> makeManagerAvailableUnavailable(int? id)
        {
            if (id == null || _context.managers == null)
            {
                return NotFound();
            }

            var manager = await _context.managers.Include("applicationUser").Where(e => e.Id == id).FirstOrDefaultAsync();
            if (manager == null)
            {
                return NotFound();
            }
            if (manager.applicationUser.available == true)
            {

                manager.applicationUser.available = false;
                var userTask = _userManager.FindByEmailAsync(manager.applicationUser.Email);
                userTask.Wait();
                var user = userTask.Result;
                var lockUserTask = _userManager.SetLockoutEnabledAsync(user, true);
                lockUserTask.Wait();
                var lockDateTask = _userManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);
                lockDateTask.Wait();
            }
            else
            {
                manager.applicationUser.available = true;
                var userTask = _userManager.FindByEmailAsync(manager.applicationUser.Email);
                userTask.Wait();
                var user = userTask.Result;
                var lockDisabledTask = _userManager.SetLockoutEnabledAsync(user, false);
                lockDisabledTask.Wait();
                var setLockoutEndDateTask = _userManager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromMinutes(1));
                setLockoutEndDateTask.Wait();
            }
            try
            {
                _context.Update(manager);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(manager.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(ListCompanyEmployees));
        }



        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,classification,available")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }
            var managers = await _context.managers.Where(e => e.CompanyId == company.Id).ToListAsync();
            var employers = await _context.employees.Where(e => e.CompanyId == company.Id).ToListAsync();
            var vehicles = await _context.vehicles.Where(e => e.CompanyId == company.Id).ToListAsync();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                    if (company.available == false)
                    {

                        if (employers != null)
                        {
                            foreach (var item in employers)
                            {
                                await makeEmployeeAvailableUnavailable(item.Id);
                            }
                        }
                        if (managers != null)
                        {
                            foreach (var item in managers)
                            {
                                await makeManagerAvailableUnavailable(item.Id);
                            }
                        }
                        if (vehicles != null)
                        {
                            foreach (var item in vehicles)
                            {
                                item.available = false;
                                _context.Update(item);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {
                        if (employers != null)
                        {
                            foreach (var item in employers)
                            {
                                await makeEmployeeAvailableUnavailable(item.Id);
                            }
                        }
                        if (managers != null)
                        {
                            foreach (var item in managers)
                            {
                                await makeManagerAvailableUnavailable(item.Id);
                            }
                        }
                        if (vehicles != null)
                        {
                            foreach (var item in vehicles)
                            {
                                item.available = true;
                                _context.Update(item);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.companies == null)
            {
                return NotFound();
            }

            var company = await _context.companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.companies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.companies'  is null.");
            }
            var company = await _context.companies.FindAsync(id);
            var managers = await _context.managers.Include("applicationUser").Where(e => e.CompanyId == company.Id).ToListAsync();
            var employers = await _context.employees.Include("applicationUser").Where(e => e.CompanyId == company.Id).ToListAsync();
            var vehicles = await _context.vehicles.Where(e => e.CompanyId == company.Id).ToListAsync();

            if (company != null && vehicles.Count == 0)
            {
                if (managers != null)
                {
                    foreach (var manager in managers)
                    {
                        _context.managers.Remove(manager);
                        await DeleteUser(manager.applicationUser.Id);
                    }
                }
                if (employers != null)
                {
                    foreach (var employee in employers)
                    {
                        _context.employees.Remove(employee);
                        await DeleteUser(employee.applicationUser.Id);
                    }
                }
                _context.companies.Remove(company);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return _context.companies.Any(e => e.Id == id);
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            using (var transaction = _context.Database.BeginTransaction())
            {
                if (roles.Count > 0)
                {
                    foreach (var role in roles.ToList())
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }
                await _userManager.DeleteAsync(user);
                transaction.Commit();
            }
            return Ok();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}
