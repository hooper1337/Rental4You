using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;
namespace Rental4You.Controllers
{

    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public ReservationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Client")]
        public IActionResult Request()
        {
            ViewData["CarList"] = new SelectList(_context.vehicles.Where(v => v.available == true).ToList(), "Id", "brand");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public IActionResult Calculate([Bind("BeginDate,EndDate,vehicleId")] ReservationsViewModel request)
        {
            // ViewData["Vehicle"]
            ViewData["CarList"] = new SelectList(_context.vehicles.Where(v => v.available == true).ToList(), "Id", "brand");

            double NrDays = 0;

            if (request.BeginDate > request.EndDate)
                ModelState.AddModelError("BeginDate", "The start date cannot be greater than the end date");

            var vehicle = _context.vehicles.Include("company").Include("reservations").FirstOrDefault(v => v.Id == request.vehicleId);
            if (vehicle == null)
            {
                ModelState.AddModelError("vehicleId", "Invalid chosen vehicle");
            }

            bool available = true;
            // Iterate through each reservation for this vehicle
            foreach (Reservation reservation in vehicle.reservations)
            {
                // Check if the time frame of this reservation overlaps with the time frame we're searching for
                if ((reservation.BeginDate <= request.EndDate && reservation.EndDate >= request.BeginDate) ||
                    (reservation.EndDate >= request.BeginDate && reservation.BeginDate <= request.EndDate))
                {
                    available = false;
                    break;
                }
            }
            // If the vehicle is not available, remove it from the filtered search results
            if (!available)
            {
                ModelState.AddModelError("BeginDate", "Vehicle already has reservations for choosen time period");
            }

            if (ModelState.IsValid)
            {
                NrDays = (request.EndDate - request.BeginDate).TotalDays;

                Reservation x = new Reservation();
                x.EndDate = request.EndDate;
                x.BeginDate = request.BeginDate;
                x.vehicleId = request.vehicleId;

                x.Price = Math.Round(vehicle.costPerDay * (decimal)NrDays);
                x.vehicle = vehicle;
                x.confirmed = false;

                return View("RequestConfirmation", x);

            }

            return View("request", request);
        }

        // GET: reservations
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Index()
        {
            // My reservations
            var reservations = _context.reservations.
            Include(a => a.vehicle).
            Include(a => a.ApplicationUser).
            Where(a => a.ApplicationUserID == _userManager.GetUserId(User));
            return View(await reservations.ToListAsync());
        }

        // GET
        [Authorize(Roles = "Employer, Manager")]
        public async Task<IActionResult> ListCompanyReservations(string? error)
        {
            var applicationUserId = _userManager.GetUserId(User);

            List<Reservation> reservations;
            if (HttpContext.User.IsInRole("Employer"))
            {
                var employee = _context.employees.Where(e => e.applicationUser.Id == applicationUserId).FirstOrDefault();
                reservations = _context.reservations.Include(v => v.vehicle).Include(a => a.ApplicationUser).Where(r => r.vehicle.CompanyId == employee.CompanyId).ToList();
            }
            else
            {
                var manager = _context.managers.Where(m => m.applicationUser.Id == applicationUserId).FirstOrDefault();
                reservations = _context.reservations.Include(v => v.vehicle).Include(a => a.ApplicationUser).Where(r => r.vehicle.CompanyId == manager.CompanyId).ToList();
            }

            var model = new ReservationsViewModel
            {
                ReservationsList = reservations
            };

            return View(model);
        }


        /*
            ViewData["ErrorMessage"] = error;

            var uniqueVehiclesPlace = from p in _context.vehicles
                                      group p by new { p.place } //or group by new {p.Id, p.Whatever}
                                      into mygroup
                                      select mygroup.FirstOrDefault();
            ViewData["VehicleCategories"] = new SelectList(uniqueVehiclesPlace.ToList(), "Id", "place");

            ViewData["Client"] = new SelectList(_context.categories.ToList(), "Id", "name");
            // ViewData["withdrawDateList"] = new SelectList(_context.vehicles.ToList(), "Id", "withdrawDate"); // need to change to withdrawDate
         */

        // POST
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListCompanyReservations()
        {
            return View();

        }*/

        // GET: Agendamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.reservations
                .Include(a => a.vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Agendamentos/Create
        [Authorize(Roles = "Client")]
        public IActionResult Create()
        {
            ViewData["CarList"] = new SelectList(_context.vehicles.Where(v => v.available == true).ToList(), "Id", "brand");
            return View();
        }

        // POST: Agendamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create(
            [Bind("Id,BeginDate,EndDate,Price,DateTimeOfRequest,vehicleId")] Reservation reserv)
        {
            // clear the stuff you didn't bind?
            ModelState.Remove(nameof(reserv.vehicle));
            ModelState.Remove(nameof(reserv.ApplicationUserID));
            ModelState.Remove(nameof(reserv.ApplicationUser));

            reserv.ApplicationUserID = _userManager.GetUserId(User);
            reserv.DateTimeOfRequest = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(reserv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["TipoDeAulaId"] = new SelectList(_context.vehicles, "Id", "Id", reserv.vehicleId); // _context.TipoDeAula?
            ViewData["CarList"] = new SelectList(_context.vehicles.Where(v => v.available == true).ToList(), "Id", "brand", reserv.vehicleId);
            return View(reserv);
        }

        [Authorize(Roles = "Employer, Manager")]
        public async Task<IActionResult> ConfirmReservation(int Id)
        {
            var reservation = _context.reservations.Where(r => r.Id == Id).FirstOrDefault();
            if(ModelState.IsValid)
            {
                if(reservation != null)
                {
                    reservation.confirmed = true;
                    try 
                    {
                        _context.Update(reservation);
                        await _context.SaveChangesAsync();
                    }
                    catch(DbUpdateConcurrencyException)
                    {
                        if(!ReservationExists(reservation.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(ListCompanyReservations));
                }
            }
            return View(reservation);
        }

        // GET: Agendamentos/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CarList"] = new SelectList(_context.vehicles.ToList(), "Id", "brand", reservation.vehicleId);
            return View(reservation);
        }

        // POST: Agendamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,BeginDate,EndDate,Price,DateTimeOfRequest,vehicleId")] Reservation reserv)
        {
            if (id != reserv.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reserv.Id))
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
            ViewData["CarList"] = new SelectList(_context.vehicles.ToList(), "Id", "brand", reserv.vehicleId);
            return View(reserv);
        }

        // GET: Agendamentos/Edit/5
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> VehicleState(int? id)
        {
            if (id == null || _context.reservations == null)
            {
                return NotFound();
            }

            var res = await _context.reservations.FindAsync(id);
            if (res == null)
            {
                return NotFound();
            }
            return View(res);
        }

        // POST: Agendamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> VehicleState(int id, [Bind("Id,NumberOfKmOfVehicleDelivery,DamageDelivery,ObservationsDelivery,EmployerDelivery,DeliveryDate,NumberOfKmOfVehicleRetrieval,DamageRetrieval,ObservationsRetrieval,EmployerRetrieval,RetrievalDate")] Reservation reserv)
        {
            if (id != reserv.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    var reservation = _context.reservations.Find(id);
                    if (reservation == null)
                    {
                        return NotFound();
                    }

                    if (reserv.NumberOfKmOfVehicleDelivery != null)
                    {
                        _context.Entry(reservation).Property(r => r.NumberOfKmOfVehicleDelivery).CurrentValue = reserv.NumberOfKmOfVehicleDelivery;
                    }
                    if (reserv.DamageDelivery != null)
                    {
                        _context.Entry(reservation).Property(r => r.DamageDelivery).CurrentValue = reserv.DamageDelivery;
                    }
                    if (reserv.ObservationsDelivery != null)
                    {
                        _context.Entry(reservation).Property(r => r.ObservationsDelivery).CurrentValue = reserv.ObservationsDelivery;
                    }
                    if (reserv.EmployerDelivery != null)
                    {
                        _context.Entry(reservation).Property(r => r.EmployerDelivery).CurrentValue = reserv.EmployerDelivery;
                    }
                    if (reserv.DeliveryDate != null)
                    {
                        _context.Entry(reservation).Property(r => r.DeliveryDate).CurrentValue = reserv.DeliveryDate;
                    }
                    if (reserv.NumberOfKmOfVehicleRetrieval != null)
                    {
                        _context.Entry(reservation).Property(r => r.NumberOfKmOfVehicleRetrieval).CurrentValue = reserv.NumberOfKmOfVehicleRetrieval;
                    }
                    if (reserv.DamageRetrieval != null)
                    {
                        _context.Entry(reservation).Property(r => r.DamageRetrieval).CurrentValue = reserv.DamageRetrieval;
                    }
                    if (reserv.ObservationsRetrieval != null)
                    {
                        _context.Entry(reservation).Property(r => r.ObservationsRetrieval).CurrentValue = reserv.ObservationsRetrieval;
                    }
                    if (reserv.EmployerRetrieval != null)
                    {
                        _context.Entry(reservation).Property(r => r.EmployerRetrieval).CurrentValue = reserv.EmployerRetrieval;
                    }
                    if (reserv.RetrievalDate != null)
                    {
                        _context.Entry(reservation).Property(r => r.RetrievalDate).CurrentValue = reserv.RetrievalDate;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListCompanyReservations));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reserv.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                //}
            }
            ViewData["CarList"] = new SelectList(_context.vehicles.ToList(), "Id", "brand", reserv.vehicleId);
            return View(reserv);
        }



        // GET: Agendamentos/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.reservations == null)
            {
                return NotFound();
            }

            var agendamento = await _context.reservations
                .Include(a => a.vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agendamento == null)
            {
                return NotFound();
            }

            return View(agendamento);
        }

        // POST: Agendamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.reservations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Agendamentos'  is null.");
            }
            var reservation = await _context.reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> RejectReservation(int id)
        {
            if (_context.reservations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Agendamentos'  is null.");
            }
            var reservation = await _context.reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListCompanyReservations));
        }

        private bool ReservationExists(int id)
        {
            return _context.reservations.Any(e => e.Id == id);
        }
    }
}
