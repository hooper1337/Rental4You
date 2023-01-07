using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Migrations;
using Rental4You.Models;
using System.Data;
using System.Security.Claims;
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

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            
            return View();
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

        public IActionResult GetDataReservations()
        {
            //dados de exemplo
            List<object> data = new List<object>();


            DateTime currentDate = DateTime.Now;
            DateTime last30Days = currentDate.AddDays(-30);

            var reservationsLast30Days = _context.reservations.Include("vehicle")
                                                              .Where(r => r.DateTimeOfRequest > last30Days
                                                                && r.DateTimeOfRequest < currentDate
                                                                )
                                                              .ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Days", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Quantity", System.Type.GetType("System.Int32"));
            foreach (DateTime day in EachDay(last30Days, DateTime.Now))
            {
                DataRow dr = dt.NewRow();
                dr["Days"] = day.Day;
                var reservations = reservationsLast30Days.Where(r => r.DateTimeOfRequest.Day == day.Day).ToList();
                var quantity = 0;
                if (reservations != null)
                {
                    quantity = reservations.Count;
                }
                dr["Quantity"] = quantity;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                data.Add(x);
            }
            return Json(data);
        }

        public IActionResult GetDataReservationsMonth()
        {
            List<object> data = new List<object>();
            DateTime currentDate = DateTime.Now;
            DateTime lastYear = currentDate.AddYears(-1);

            var reservationsLastYear = _context.reservations.Include("vehicle")
                                                              .Where(r => r.DateTimeOfRequest > lastYear
                                                                && r.DateTimeOfRequest < currentDate
                                                                )
                                                              .ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Month", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Quantity", System.Type.GetType("System.Int32"));

            foreach (DateTime day in EachMonth(lastYear, DateTime.Now))
            {
                DataRow dr = dt.NewRow();
                dr["Month"] = day.Month;
                var reservations = reservationsLastYear.Where(r => r.DateTimeOfRequest.Month == day.Month && r.DateTimeOfRequest.Year == day.Year).ToList();
                var quantity = 0;
                if (reservations != null)
                {
                    quantity = reservations.Count;
                }
                dr["Quantity"] = quantity;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                data.Add(x);
            }
            return Json(data);
        }

        public IActionResult GetDataUsersMonth()
        {
            List<object> data = new List<object>();
            DateTime currentDate = DateTime.Now;
            DateTime lastYear = currentDate.AddYears(-1);

            var userLastYear = _context.Users.Where(u => u.registerDate > lastYear
                                                                && u.registerDate < currentDate
                                                                )
                                                              .ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Month", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Quantity", System.Type.GetType("System.Int32"));

            foreach (DateTime day in EachMonth(lastYear, DateTime.Now))
            {
                DataRow dr = dt.NewRow();
                dr["Month"] = day.Month;
                var users = userLastYear.Where(r => r.registerDate?.Month == day.Month && r.registerDate?.Year == day.Year).ToList();
                var quantity = 0;
                if (users != null)
                {
                    quantity = users.Count;
                }
                dr["Quantity"] = quantity;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                data.Add(x);
            }
            return Json(data);
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
        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public IEnumerable<DateTime> EachMonth(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddMonths(1))
                yield return day;
        }

    }
}
