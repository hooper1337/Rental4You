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
        public async Task<IActionResult> ListCompanyReservations(
            string? error, 
            [Bind("ApplicationUserID,CategoryId,BeginDate,EndDate")] ReservationsViewModel searchReservations
            )
        {
            var applicationUserId = _userManager.GetUserId(User);

            var me1 = searchReservations.ApplicationUserID;
            var me2 = searchReservations.CategoryId;
            var me3= searchReservations.BeginDate;
            var me4 = searchReservations.EndDate;

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
            // -------------- Check for erros in the dates --------------
            if (((searchReservations.BeginDate == null || searchReservations.BeginDate == default(DateTime)) && (searchReservations.EndDate != null && searchReservations.EndDate != default(DateTime))) ||
            ((searchReservations.BeginDate != null && searchReservations.BeginDate != default(DateTime)) && (searchReservations.EndDate == null || searchReservations.EndDate == default(DateTime))))
            {

                ViewData["ErrorMessage"] = "If you specify a date, you need to specify them both";
                return View(model);
            }
            if (searchReservations.EndDate < searchReservations.BeginDate)
            {

                ViewData["ErrorMessage"] = "The end date must be after the start date.";
                return View(model);
            }
            // -------------- Check for erros in the dates --------------


            if (!(searchReservations.CategoryId == 0 || searchReservations.CategoryId == null))
            {
                reservations = reservations.Where(r => r.vehicle.CategoryId == searchReservations.CategoryId).ToList();
            }
            if (!string.IsNullOrEmpty(searchReservations.ApplicationUserID))
            {
                reservations = reservations.Where(r => r.ApplicationUser.Id == searchReservations.ApplicationUserID).ToList();
            }


            if (searchReservations.BeginDate != default(DateTime) && searchReservations.EndDate != default(DateTime))
            {

                // Check the dates
                //IQueryable<Reservation> filteredSearchResults = searchReservations;
                List<Reservation> reservationsToRemove = new List<Reservation>();

                foreach (Reservation reservation in reservations)
                {
                    bool available = false;

                    // Check if the time frame of this reservation overlaps with the time frame we're searching for
                    if (reservation.BeginDate <= searchReservations.EndDate && reservation.BeginDate >= searchReservations.BeginDate && reservation.EndDate <= searchReservations.EndDate)
                    {
                        available = true;
                    }

                    if (!available)
                    {
                        // Add reservation to the list of reservations to remove
                        reservationsToRemove.Add(reservation);
                    }

                }
                // Remove reservations from the original list
                foreach (Reservation reservation in reservationsToRemove)
                {
                    reservations.Remove(reservation);
                }

            }

            var modelFiltered = new ReservationsViewModel
            {
                ReservationsList = reservations
            };


            var uniqueVehicleCategories = (from v in _context.vehicles
                                           select v.category).Distinct();
            ViewData["VehicleCategories"] = new SelectList(uniqueVehicleCategories.ToList(), "Id", "name");

            var uniqueClients = (from r in _context.reservations
                                select r.ApplicationUser).Distinct();
            ViewData["Client"] = new SelectList(uniqueClients.ToList(), "Id", "firstName");

            return View(modelFiltered);
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

            var reservation = await _context.reservations // .Include("vehicleStateDelivery").Include("vehicleStateRetrieval")
                .Include(a => a.vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.vehicleStateDelivery = _context.vehicleStates.Find(reservation.vehicleStateDeliveryId);
            reservation.vehicleStateRetrieval = _context.vehicleStates.Find(reservation.vehicleStateRetrievalId);


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
        [Authorize(Roles = "Employer, Manager")]
        public async Task<IActionResult> VehicleState(int? id)
        {
            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

            var employers = await userManager.GetUsersInRoleAsync("Employer");

            ViewData["Employers"] = new SelectList(employers, "Id", "firstName");

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
        [Authorize(Roles = "Employer, Manager")]
        public async Task<IActionResult> VehicleState(
            int id, 
            [Bind("Id,BeginDate,EndDate,Price,DateTimeOfRequest,vehicleId,vehicle,ApplicationUserID,confirmed,ApplicationUser,vehicleStateDelivery,vehicleStateRetrieval")] Reservation reserv, 
            [Bind("NumberOfKmOfVehicle,Damage,Observations,ApplicationUserID")] VehicleState vehicleStateDelivery, 
            [Bind("NumberOfKmOfVehicle,Damage,Observations,ApplicationUserID")] VehicleState vehicleStateRetrieval
        )
        {
            if (id != reserv.Id)
            {
                return NotFound();
            }

            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

            var employers = await userManager.GetUsersInRoleAsync("Employer");

            ViewData["Employers"] = new SelectList(employers, "Id", "firstName");


            IQueryable<Reservation> searchResults = _context.reservations.Include("vehicleStates"); // .Include("categoria")

            reserv = _context.reservations.Find(reserv.Id);
            reserv.vehicleStateDelivery = vehicleStateDelivery;
            reserv.vehicleStateRetrieval = vehicleStateRetrieval;

            if (reserv.vehicleStateRetrieval.NumberOfKmOfVehicle != null && reserv.vehicleStateRetrieval.Observations != null &&
                reserv.vehicleStateRetrieval.ApplicationUserID != null)
            {
                // Find the vehicle in the context
                var vehicle = _context.vehicles.Find(reserv.vehicleId);

                // Set the vehicleStateId property of the vehicle
                vehicle.vehicleStateId = reserv.vehicleStateRetrievalId;

                // Save the changes to the database
                _context.SaveChanges();
            } else
            {
                // Find the vehicle in the context
                var vehicle = _context.vehicles.Find(reserv.vehicleId);

                // Set the vehicleStateId property of the vehicle
                vehicle.vehicleStateId = reserv.vehicleStateDeliveryId;

                // Save the changes to the database
                _context.SaveChanges();
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
                return RedirectToAction(nameof(ListCompanyReservations));
            } else
            {
                String myerror = "";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        // do something with the error message
                        myerror += error.ErrorMessage + '\n';
                    }
                }
                ModelState.AddModelError("ErrorMessage", myerror);
                ViewData["ErrorMessage"] = myerror;

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
