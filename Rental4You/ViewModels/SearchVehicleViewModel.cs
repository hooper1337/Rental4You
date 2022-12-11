using Rental4You.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.ViewModels
{
    public class SearchVehicleViewModel
    {
        public List<Vehicle> VehicleList { get; set; }
        public int NumResults { get; set; }
        [Display(Name = "SEARCH FOR VEHICLES ...", Prompt = "Incert text to search...")]
        public string TextToSearch { get; set; }

        public int Order { get; set; }

    }
}
