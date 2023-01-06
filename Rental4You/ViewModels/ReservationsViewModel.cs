using Rental4You.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Rental4You.ViewModels
{
    public class ReservationsViewModel
    {
        public List<Reservation>? ReservationsList { get; set; }

        [Display(Name = "Begin Date", Prompt = "yyyy-mm-dd")]
        public DateTime BeginDate { get; set; }
        [Display(Name = "End date", Prompt = "yyyy-mm-dd")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Vehicle", Prompt = "Choose the vehicle")]
        public int vehicleId { get; set; }
        [Display(Name = "CategoryId", Prompt = "category of vehicle but its the ID!")]
        public int? CategoryId { get; set; }

        [Display(Name = "Category", Prompt = "category of vehicle")]
        public Category? Category { get; set; }

        [Display(Name = "Client")]
        public string? ApplicationUserID { get; set; }
        public ApplicationUser? client { get; set; }
    }
}
