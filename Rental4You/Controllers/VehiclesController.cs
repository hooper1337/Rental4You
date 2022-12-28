using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;

namespace Rental4You.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

            public VehiclesController(ApplicationDbContext context)
            {
                _context = context;
            }

        // ---------- Search ----------
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Index(
            string? TextToSearch, 
            [Bind("TextToSearch,Order,BeginDateSearch,EndDateSearch")] SearchVehicleViewModel pesquisaCurso,
            [Bind("Id,brand,model,type,CompanyId,place,withdraw,deliver,costPerDay")] Vehicle vehicle
            )
        {

            if ( // if not both are filled
                    ( pesquisaCurso.BeginDateSearch == default(DateTime) && pesquisaCurso.EndDateSearch != default(DateTime) ) ||
                    ( pesquisaCurso.BeginDateSearch != default(DateTime) && pesquisaCurso.EndDateSearch == default(DateTime) )
                )
            {
                ModelState.AddModelError("BeginDateSearch", "Both start and end dates must be specified.");
                ModelState.AddModelError("EndDateSearch", "Both start and end dates must be specified.");

                return RedirectToAction("Index", "Home", new { error = "If you specify a date, you need to specify them both" });
            }

            if (pesquisaCurso.EndDateSearch < pesquisaCurso.BeginDateSearch)
            {
                ModelState.AddModelError("BeginDateSearch", "End Date must be set after BeginDate");
                ModelState.AddModelError("EndDateSearch", "End Date must be set after BeginDate");

                return RedirectToAction("Index", "Home", new { error = "The end date must be after the start date." });
            }


            IQueryable<Vehicle> searchResults = _context.vehicles.Include("company").Include("reservations"); // .Include("categoria")

            if (string.IsNullOrWhiteSpace(TextToSearch)) {
                //IQueryable<Vehicle> searchResults = _context.vehicles.Include("company").Include("reservations"); // .Include("categoria")
                //pesquisaCurso.VehicleList = await searchResults.ToListAsync();
            }
            else
            {
                searchResults = _context.vehicles.Include("company").Include("reservations"). // Include("categoria").
                    Where(c => 
                                c.type.Contains(TextToSearch) ||
                                c.place.Contains(TextToSearch) ||
                                c.costPerDay.ToString().Contains(TextToSearch) ||
                                c.brand.Contains(TextToSearch) ||
                                c.company.name.Contains(TextToSearch)
                         );

                pesquisaCurso.TextToSearch = TextToSearch;
            }

            if (vehicle.place != null) {
                // vehicle.place only has the ID of the vehicle that has the place we want to show
                var placeToSearch = _context.vehicles.Find(Convert.ToInt32(vehicle.place)).place;
                searchResults = searchResults. // Include("categoria").
                    Where(c => c.place.Equals(placeToSearch));
            }
            if (vehicle.type != null)
            {
                var typeToSearch = _context.vehicles.Find(Convert.ToInt32(vehicle.type)).type;
                searchResults = searchResults. // Include("categoria").
                    Where(c => c.type.Equals(typeToSearch));
            }

            // Check the dates
            IQueryable<Vehicle> filteredSearchResults = searchResults;

            foreach (Vehicle veh in searchResults)
            {
                bool available = true;

                // Iterate through each reservation for this vehicle
                foreach (Reservation reservation in veh.reservations)
                {
                    // Check if the time frame of this reservation overlaps with the time frame we're searching for
                    if ((reservation.BeginDate <= pesquisaCurso.EndDateSearch && reservation.EndDate >= pesquisaCurso.BeginDateSearch) ||
                        (reservation.EndDate >= pesquisaCurso.BeginDateSearch && reservation.BeginDate <= pesquisaCurso.EndDateSearch))
                    {
                        available = false;
                        break;
                    }
                }

                // If the vehicle is not available, remove it from the filtered search results
                if (!available)
                {
                    filteredSearchResults = filteredSearchResults.Where(v => v.Id != veh.Id);
                }
            }
            searchResults = filteredSearchResults;


            pesquisaCurso.VehicleList = await searchResults.ToListAsync();
            pesquisaCurso.NumResults = pesquisaCurso.VehicleList.Count();

            if (pesquisaCurso.Order == 1)
                pesquisaCurso.VehicleList = pesquisaCurso.VehicleList.OrderBy(v => v.costPerDay).ToList();
            if (pesquisaCurso.Order == 2)
                pesquisaCurso.VehicleList = pesquisaCurso.VehicleList.OrderByDescending(v => v.costPerDay).ToList();
            if (pesquisaCurso.Order == 3)
                pesquisaCurso.VehicleList = pesquisaCurso.VehicleList.OrderBy(v => v.company.classification).ToList();
            if (pesquisaCurso.Order == 4)
                pesquisaCurso.VehicleList = pesquisaCurso.VehicleList.OrderByDescending(v => v.company.classification).ToList();

            return View(pesquisaCurso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Index(
            [Bind("TextToSearch,Order,StartDate,BeginDateSearch,EndDateSearch")]
            SearchVehicleViewModel pesquisaCurso
            )
        {

            if (string.IsNullOrEmpty(pesquisaCurso.TextToSearch))
            {
                pesquisaCurso.VehicleList =
                    await _context.vehicles.Include("company").Include("reservations").ToListAsync(); // .Include("categoria")

                pesquisaCurso.NumResults = pesquisaCurso.VehicleList.Count();
            }
            else
            {
                pesquisaCurso.VehicleList =
                    await _context.vehicles.Include("company").Include("reservations").Where( // Include(c => c.categoria).
                        c => c.brand.Contains(pesquisaCurso.TextToSearch) ||
                                c.model.Contains(pesquisaCurso.TextToSearch) ||
                                c.type.Contains(pesquisaCurso.TextToSearch) ||
                                c.place.Contains(pesquisaCurso.TextToSearch) ||
                                c.costPerDay.ToString().Contains(pesquisaCurso.TextToSearch)
                         
                        ).ToListAsync();

                pesquisaCurso.NumResults = pesquisaCurso.VehicleList.Count();

            }

            if (pesquisaCurso.Order == 1)
                pesquisaCurso.VehicleList = pesquisaCurso.VehicleList.OrderBy(v => v.costPerDay).ToList();
            if (pesquisaCurso.Order == 2)
                pesquisaCurso.VehicleList = pesquisaCurso.VehicleList.OrderByDescending(v => v.costPerDay).ToList();

            return View(pesquisaCurso);
        }
        // ---------- Search ----------

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.vehicles.Include("company").Include("reservations")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> CompanyVehicles()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.employees.Where(e => e.applicationUser.Id == applicationUserId).FirstOrDefault();
            var vehicles = await _context.vehicles.Include("company").Where(v => v.CompanyId == employee.CompanyId).ToListAsync();
            return View(vehicles);
        }

        // GET: Vehicles/Create
        [Authorize(Roles = "Employer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Create([Bind("Id,brand,model,type,place,withdraw,deliver,costPerDay,available")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var employee = _context.employees.Where(e => e.applicationUser.Id == applicationUserId).FirstOrDefault();
                vehicle.CompanyId = employee.CompanyId;
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CompanyVehicles));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,brand,model,type,place,withdraw,deliver,costPerDay,available")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(vehicle.company));
            if (ModelState.IsValid)
            {
                try
                {
                    var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var employee = _context.employees.Where(e => e.applicationUser.Id == applicationUserId).FirstOrDefault();
                    vehicle.CompanyId = employee.CompanyId;
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CompanyVehicles));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.vehicles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.vehicles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.vehicles'  is null.");
            }

            var vehicle = await _context.vehicles.FindAsync(id);
            var vehiclesReservation = _context.reservations.Include("vehicle").Where(v => v.vehicleId == vehicle.Id).FirstOrDefault();
            if (vehicle != null && vehiclesReservation == null)
            {
                _context.vehicles.Remove(vehicle);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CompanyVehicles));
        }

        private bool VehicleExists(int id)
        {
          return _context.vehicles.Any(e => e.Id == id);
        }
    }
}
