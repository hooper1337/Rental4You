using Microsoft.AspNetCore.Mvc;
using Rental4You.Models;
using System.Diagnostics;

namespace Rental4You.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var uniqueVehiclesPlace = from p in _context.vehicles
                                      group p by new { p.place } //or group by new {p.Id, p.Whatever}
                                      into mygroup
                                      select mygroup.FirstOrDefault();
            ViewData["LocationList"] = new SelectList(uniqueVehiclesPlace.ToList(), "Id", "place");

            var uniqueVehiclesTypes = from p in _context.vehicles
                               group p by new { p.type }
                               into mygroup
                               select mygroup.FirstOrDefault();
            ViewData["TypeList"] = new SelectList(uniqueVehiclesTypes.ToList(), "Id", "type");
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