using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Display(Name = "Name", Prompt = "Insert the comapany name...")]
        public string name { get; set; }
        [Display(Name = "Classification", Prompt = "Insert vehicle brand...")]
        public int classification { get; set; } // from 0 to 10
        [Display(Name = "Car List")]
        public ICollection<Vehicle> vehicles { get; set; }
        public ICollection<Employee> employers { get; set; }
    }
}
