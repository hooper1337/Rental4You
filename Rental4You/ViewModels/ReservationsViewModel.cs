using Rental4You.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Rental4You.ViewModels
{
    public class ReservationsViewModel
    {
        [Display(Name = "Name Cliente", Prompt = "Incert name of the client")]
        public string Cliente { get; set; }
        [Display(Name = "Begin Date", Prompt = "yyyy-mm-dd")]
        public DateTime BeginDate { get; set; }
        [Display(Name = "End date", Prompt = "yyyy-mm-dd")]
        public DateTime EndDate { get; set; }

        [Display(Name = "vehicle", Prompt = "Choose the vehicle")]
        public int vehicleId { get; set; }
    }
}
