using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            return View(await _context.vehicles.ToListAsync());
        }


        // ---------- Search ----------
        public async Task<IActionResult> Search(
            string? TextToSearch, 
            [Bind("TextToSearch,Order")] SearchVehicleViewModel pesquisaCurso)
        {
            IQueryable<Vehicle> searchResults = _context.vehicles.Include("company"); // .Include("categoria")

            if (string.IsNullOrWhiteSpace(TextToSearch)) {
                IQueryable<Vehicle> searchResults = _context.vehicles.Include("company"); // .Include("categoria")
                pesquisaCurso.VehicleList = await searchResults.ToListAsync();
            }
            else
            {
                pesquisaCurso.VehicleList = await _context.vehicles. // Include("categoria").
                    Where(c => 
                                c.type.Contains(TextToSearch) ||
                                c.place.Contains(TextToSearch) ||
                                c.costPerDay.ToString().Contains(TextToSearch)
                         ).ToListAsync();

                pesquisaCurso.TextToSearch = TextToSearch;
            }

            pesquisaCurso.NumResults = pesquisaCurso.VehicleList.Count();

            /*  <option value="1">Lowest Price</option>
                <option value="2">Highest Price</option>
                <option value="3">Lowest Company Classification</option>
                <option value="4">Highest Company Classification</option> */
            if (pesquisaCurso.Order == 1)
                pesquisaCurso.VehicleList = pesquisaCurso.VehicleList.OrderBy(v => v.costPerDay).ToList();
            if (pesquisaCurso.Order == 2)
                pesquisaCurso.VehicleList = pesquisaCurso.VehicleList.OrderByDescending(v => v.costPerDay).ToList();


            pesquisaCurso.VehicleList = await searchResults.ToListAsync();
            return View(pesquisaCurso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(
            [Bind("TextToSearch,Order")]
            SearchVehicleViewModel pesquisaCurso
            )
        {

            if (string.IsNullOrEmpty(pesquisaCurso.TextToSearch))
            {
                pesquisaCurso.VehicleList =
                    await _context.vehicles.ToListAsync(); // .Include("categoria")

                pesquisaCurso.NumResults = pesquisaCurso.VehicleList.Count();
            }
            else
            {
                pesquisaCurso.VehicleList =
                    await _context.vehicles.Where( // Include(c => c.categoria).
                        c => c.brand.Contains(pesquisaCurso.TextToSearch) ||
                                c.model.Contains(pesquisaCurso.TextToSearch) ||
                                c.type.Contains(pesquisaCurso.TextToSearch) ||
                                c.place.Contains(pesquisaCurso.TextToSearch) ||
                                c.costPerDay.ToString().Contains(pesquisaCurso.TextToSearch)
                         
                        ).ToListAsync();

                pesquisaCurso.NumResults = pesquisaCurso.VehicleList.Count();

            }


            /*  <option value="1">Lowest Price</option>
                <option value="2">Highest Price</option>
                <option value="3">Lowest Company Classification</option>
                <option value="4">Highest Company Classification</option> */
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

            var vehicle = await _context.vehicles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,brand,model,type,place,withdraw,deliver,costPerDay")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
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
            ViewData["CompaniesList"] = new SelectList(_context.companies.ToList(), "Id", "name", vehicle.CompanyId);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,brand,model,type,CompanyId,place,withdraw,deliver,costPerDay")] Vehicle vehicle)
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
                    var company = _context.companies.FindAsync(vehicle.CompanyId);

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
                return RedirectToAction(nameof(Index));
            }
            ;
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.vehicles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.vehicles'  is null.");
            }
            var vehicle = await _context.vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.vehicles.Remove(vehicle);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
          return _context.vehicles.Any(e => e.Id == id);
        }
    }
}
