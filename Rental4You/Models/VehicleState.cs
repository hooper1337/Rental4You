using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class VehicleState
    {
        public int Id { get; set; }

        [Display(Name = "Number of km of vehicle")]
        public int? NumberOfKmOfVehicle { get; set; }
        [Display(Name = "Damage on Vehicle")]
        public Boolean Damage { get; set; }
        [Display(Name = "Observations")]
        public string? Observations { get; set; }
        [Display(Name = "The Employer that reported ID")]
        // The employer that reported
        public string ApplicationUserID { get; set; }
        [Display(Name = "The Employer that reported")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
