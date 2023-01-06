using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        [Display(Name = "Brand", Prompt = "Insert vehicle brand...")]
        public string brand { set; get; }
        [Display(Name = "Model", Prompt = "Insert vehicle model...")]
        public string model { set; get; }
        [Display(Name = "Type", Prompt = "Choose category for the vehicle...")]
        public int CategoryId { get; set; }
        public Category? category { set; get; }
        [Display(Name = "Place", Prompt = "Insert place for the vehicle...")]
        public string place { set; get; }
        [Display(Name = "Cost/Day", Prompt = "Choose the vehicle cost per day...")]
        public int costPerDay { set; get; }
        public int? CompanyId { get; set; }
        [Display(Name = "Company", Prompt = "Choose vehicle company...")]
        public Company? company { set; get; }
        [Display(Name = "Available", Prompt = "Is this vehicle available...")]
        public Boolean available { set; get; }
        public int? vehicleStateId { get; set; }
        [Display(Name = "Company", Prompt = "Choose vehicle State...")]
        public VehicleState? vehicleStateNow { set; get; }

        public ICollection<Reservation>? reservations { get; set; }
    }

}
