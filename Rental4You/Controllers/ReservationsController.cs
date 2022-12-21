using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;
using Rental4You.Data;
using Rental4You.Models;

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

        public async Task<IActionResult> Pedido()
        {
            ViewData["TipoDeAulaId"] = new SelectList(_context.vehicles, "Id", "Brand");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Calcular([Bind("Cliente,BeginDate,EndDate,vehicleId")] ReservationsViewModel request)
        {
            // ViewData["Vehicle"]
            ViewData["TipoDeAulaId"] = new SelectList(_context.vehicles, "Id", "Brand");

            double NrDays = 0;

            if (request.BeginDate > request.EndDate)
                ModelState.AddModelError("BeginDate", "The start date cannot be greater than the end date");

            var vehicle = _context.vehicles.Find(request.vehicleId);
            if (vehicle == null)
            {
                ModelState.AddModelError("TipoDeAulaId", "Invalid chosen vehicle");
            }

            if (ModelState.IsValid)
            {
                NrDays = (request.EndDate - request.BeginDate).TotalDays;

                Reservation x = new Reservation();
                // x.Cliente = pedido.Cliente;
                x.EndDate = request.EndDate;
                x.BeginDate = request.BeginDate;
                x.vehicleId = request.vehicleId;

                x.Price = vehicle.costPerDay * (decimal)NrDays;
                x.vehicle = vehicle;

                return View("PedidoConfirmacao", x);

            }

            return View("pedido", request);
        }

        // GET: Agendamentos
        [Authorize] // só utilizadores autenticados têm acesso ao controler
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.reservations.Include(a => a.vehicle);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Agendamentos/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Agendamentos/Create
        [Authorize(Roles = "Cliente")]
        public IActionResult Create()
        {
            ViewData["TipoDeAulaId"] = new SelectList(_context.vehicles, "Id", "Id"); //???
            return View();
        }

        // POST: Agendamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
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
            ViewData["TipoDeAulaId"] = new SelectList(_context.vehicles, "Id", "Id", reserv.vehicleId); // _context.TipoDeAula?
            return View(reserv);
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
            ViewData["TipoDeAulaId"] = new SelectList(_context.vehicles, "Id", "Id", reservation.vehicleId);
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
            ViewData["TipoDeAulaId"] = new SelectList(_context.vehicles, "Id", "Id", reserv.vehicleId);
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

        // Get Agendamentos
        [Authorize]
        public async Task<IActionResult> MyReservations() // Vai buscar o nas views por defeito a view com o mesmo nome do método
        {
            var agendamentos = _context.reservations.
                Include(a => a.vehicle).
                Include(a => a.ApplicationUser).
                Where(a => a.ApplicationUserID == _userManager.GetUserId(User));

            return View(await agendamentos.ToListAsync());
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
            var agendamento = await _context.reservations.FindAsync(id);
            if (agendamento != null)
            {
                _context.reservations.Remove(agendamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.reservations.Any(e => e.Id == id);
        }
    }
}
