using Rental4You.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Rental4You.ViewModels
{
    public class ReservationsViewModel
    {
        [Display(Name = "Begin Date", Prompt = "yyyy-mm-dd")]
        public DateTime BeginDate { get; set; }
        [Display(Name = "End date", Prompt = "yyyy-mm-dd")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Vehicle", Prompt = "Choose the vehicle")]
        public int vehicleId { get; set; }
    }
}
