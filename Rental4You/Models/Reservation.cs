using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public DateTime DateTimeOfRequest { get; set; }
        public int vehicleId { get; set; }
        public Vehicle vehicle { get; set; }

        // relacionamento com a endtidade ApplicationUser
        public string ApplicationUserID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        
    }
}
