using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using System.Diagnostics;

namespace Rental4You.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index(string? error)
        {
            ViewData["ErrorMessage"] = error;

            var uniqueVehiclesPlace = from p in _context.vehicles
                                      group p by new { p.place } //or group by new {p.Id, p.Whatever}
                                      into mygroup
                                      select mygroup.FirstOrDefault();
            ViewData["LocationList"] = new SelectList(uniqueVehiclesPlace.ToList(), "Id", "place");

            ViewData["CategoryList"] = new SelectList(_context.categories.ToList(), "Id", "name");
            // ViewData["withdrawDateList"] = new SelectList(_context.vehicles.ToList(), "Id", "withdrawDate"); // need to change to withdrawDate

            return View();
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}