using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        [Display(Name = "Begin Date")]
        public DateTime BeginDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        [Display(Name = "Date of Request")]
        public DateTime DateTimeOfRequest { get; set; }

        [Display(Name = "Vehicle ID:")]
        public int vehicleId { get; set; }
        [Display(Name = "Vehicle")]
        public Vehicle vehicle { get; set; }
        public string ApplicationUserID { get; set; }
        [Display(Name = "Confirmed")]
        public bool confirmed { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
