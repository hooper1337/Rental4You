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


        [Display(Name = "Number of km of vehicle on delivery to client")]
        public int? NumberOfKmOfVehicleDelivery { get; set; }
        [Display(Name = "Damage on Vehicle On Delivery")]
        public bool? DamageDelivery { get; set; }
        [Display(Name = "Observations On Delivery")]
        public string? ObservationsDelivery { get; set; }
        [Display(Name = "The Employer that delivered")]
        public string? EmployerDelivery { get; set; }


        [Display(Name = "Number Of Km Of Vehicle on Retrieval from client")]
        public int? NumberOfKmOfVehicleRetrieval { get; set; }
        [Display(Name = "Damage on Vehicle On Retrieval")]
        public bool? DamageRetrieval { get; set; }
        [Display(Name = "Observations On Retrieval")]
        public string? ObservationsRetrieval { get; set; }
        [Display(Name = "The Employer that retrieved the vehicle")]
        public string? EmployerRetrieval { get; set; }
    }
}
