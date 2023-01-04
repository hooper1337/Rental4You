﻿using System;
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
        public async Task<IActionResult> Index(
            string? TextToSearch, 
            [Bind("TextToSearch,Order,BeginDateSearch,EndDateSearch")] SearchVehicleViewModel searchCurso,
            [Bind("Id,brand,model,CompanyId,type,CompanyId,place,withdraw,deliver,costPerDay")] Vehicle vehicle
            )
        {

            if ( // if not both are filled
                    ( searchCurso.BeginDateSearch == default(DateTime) && searchCurso.EndDateSearch != default(DateTime) ) ||
                    ( searchCurso.BeginDateSearch != default(DateTime) && searchCurso.EndDateSearch == default(DateTime) )
                )
            {
                ModelState.AddModelError("BeginDateSearch", "Both start and end dates must be specified.");
                ModelState.AddModelError("EndDateSearch", "Both start and end dates must be specified.");

                return RedirectToAction("Index", "Home", new { error = "If you specify a date, you need to specify them both" });
            }

            if (searchCurso.EndDateSearch < searchCurso.BeginDateSearch)
            {
                ModelState.AddModelError("BeginDateSearch", "End Date must be set after BeginDate");
                ModelState.AddModelError("EndDateSearch", "End Date must be set after BeginDate");

                return RedirectToAction("Index", "Home", new { error = "The end date must be after the start date." });
            }


            IQueryable<Vehicle> searchResults = _context.vehicles.Include("company").Include("reservations").Include("category"); // .Include("categoria")

            if (string.IsNullOrWhiteSpace(TextToSearch)) {
                searchResults = _context.vehicles.Include("company").Include("reservations").Include("category"); // .Include("categoria")
                searchCurso.VehicleList = await searchResults.ToListAsync();
            }
            else
            {
                searchResults = _context.vehicles.Include("company").Include("reservations").Include("category"). // Include("categoria").
                    Where(c => 
                                c.category.name.Contains(TextToSearch) ||
                                c.place.Contains(TextToSearch) ||
                                c.costPerDay.ToString().Contains(TextToSearch) ||
                                c.brand.Contains(TextToSearch) ||
                                c.company.name.Contains(TextToSearch)
                         );

                searchCurso.TextToSearch = TextToSearch;
            }

            if (vehicle.place != null) {
                // vehicle.place only has the ID of the vehicle that has the place we want to show
                var placeToSearch = _context.vehicles.Find(Convert.ToInt32(vehicle.place)).place;
                searchResults = searchResults. // Include("categoria").
                    Where(c => c.place.Equals(placeToSearch));
            }
            if (vehicle.category != null)
            {
                var typeToSearch = _context.vehicles.Find(Convert.ToInt32(vehicle.category.name)).category;
                searchResults = searchResults. // Include("categoria").
                    Where(c => c.category.name.Equals(typeToSearch));
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
                    if ((reservation.BeginDate <= searchCurso.EndDateSearch && reservation.EndDate >= searchCurso.BeginDateSearch) ||
                        (reservation.EndDate >= searchCurso.BeginDateSearch && reservation.BeginDate <= searchCurso.EndDateSearch))
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


            searchCurso.VehicleList = await searchResults.ToListAsync();
            searchCurso.NumResults = searchCurso.VehicleList.Count();

            if (searchCurso.Order == 1)
                searchCurso.VehicleList = searchCurso.VehicleList.OrderBy(v => v.costPerDay).ToList();
            if (searchCurso.Order == 2)
                searchCurso.VehicleList = searchCurso.VehicleList.OrderByDescending(v => v.costPerDay).ToList();
            if (searchCurso.Order == 3)
                searchCurso.VehicleList = searchCurso.VehicleList.OrderBy(v => v.company.classification).ToList();
            if (searchCurso.Order == 4)
                searchCurso.VehicleList = searchCurso.VehicleList.OrderByDescending(v => v.company.classification).ToList();

            return View(searchCurso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            [Bind("TextToSearch,Order,StartDate,BeginDateSearch,EndDateSearch")]
            SearchVehicleViewModel pesquisaCurso
            )
        {

            if (string.IsNullOrEmpty(pesquisaCurso.TextToSearch))
            {
                pesquisaCurso.VehicleList =
                    await _context.vehicles.Include("company").Include("reservations").Include("category").ToListAsync(); // .Include("categoria")

                pesquisaCurso.NumResults = pesquisaCurso.VehicleList.Count();
            }
            else
            {
                pesquisaCurso.VehicleList =
                    await _context.vehicles.Include("company").Include("reservations").Include("category").Where( // Include(c => c.categoria).
                        c => c.brand.Contains(pesquisaCurso.TextToSearch) ||
                                c.model.Contains(pesquisaCurso.TextToSearch) ||
                                c.category.name.Contains(pesquisaCurso.TextToSearch) ||
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

            var vehicle = await _context.vehicles.Include("company").Include("reservations").Include("category")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [Authorize(Roles = "Employer, Manager")]
        public async Task<IActionResult> CompanyVehicles()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(HttpContext.User.IsInRole("Employer"))
            {
                var employee = _context.employees.Where(e => e.applicationUser.Id == applicationUserId).FirstOrDefault();
                var vehicles = await _context.vehicles.Include("company").Include("category").Where(v => v.CompanyId == employee.CompanyId).ToListAsync();
                return View(vehicles);
            }
            var manager = _context.managers.Where(e => e.applicationUser.Id == applicationUserId).FirstOrDefault();
            var managerVehicles = await _context.vehicles.Include("company").Include("category").Where(v => v.CompanyId == manager.CompanyId).ToListAsync();
            return View(managerVehicles);
        }

        // GET: Vehicles/Create
        [Authorize(Roles = "Employer, Manager")]
        public IActionResult Create()
        {
            ViewData["CategoryList"] = new SelectList(_context.categories.ToList(), "Id", "name");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer, Manager")]
        public async Task<IActionResult> Create([Bind("Id,brand,model,CategoryId,type,place,withdraw,deliver,costPerDay,available")] Vehicle vehicle)
        {
            ViewData["CategoryList"] = new SelectList(_context.categories.ToList(), "Id", "name");
            var company = new Company();
            
            if (ModelState.IsValid)
            {
                var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (User.IsInRole("Employer"))
                {
                    var employee = _context.employees.Where(e => e.applicationUser.Id == applicationUserId).FirstOrDefault();
                    vehicle.CompanyId = employee.CompanyId;
                    company = _context.companies.Include("vehicles").Where(c => c.Id == employee.CompanyId).FirstOrDefault();
                }
                else
                {
                    var manager = _context.managers.Where(e => e.applicationUser.Id == applicationUserId).FirstOrDefault();
                    vehicle.CompanyId = manager.CompanyId;
                    company = _context.companies.Include("vehicles").Where(c => c.Id == manager.CompanyId).FirstOrDefault();
                }
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CompanyVehicles));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        [Authorize(Roles = "Employer, Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["CategoryList"] = new SelectList(_context.categories.ToList(), "Id", "name");
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
        [Authorize(Roles = "Employer, Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,brand,model,CategoryId,type,place,withdraw,deliver,costPerDay,available")] Vehicle vehicle)
        {
            ViewData["CategoryList"] = new SelectList(_context.categories.ToList(), "Id", "name");
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
                    if (vehicle.available == false)
                    {
                        var reservations = _context.reservations.Where(r => r.vehicleId == vehicle.Id && r.BeginDate > DateTime.Now).ToList();
                        if(reservations != null)
                        {
                            foreach(var reservation in reservations)
                            {
                                _context.reservations.Remove(reservation);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
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
        [Authorize(Roles = "Employer, Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.vehicles.Include("category").Where(v => v.Id == id).FirstOrDefaultAsync();

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer, Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.vehicles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.vehicles'  is null.");
            }

            var vehicle = await _context.vehicles.Include("category").Where(v => v.Id == id).FirstOrDefaultAsync();
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
