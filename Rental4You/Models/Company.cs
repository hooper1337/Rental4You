using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Display(Name = "Name", Prompt = "Insert the company name...")]
        public string name { get; set; }
        [Display(Name = "Classification", Prompt = "Insert company classification...")]
        public int classification { get; set; } // from 0 to 10

        [Display(Name = "Available", Prompt = "Insert company classification...")]
        public bool available { get; set; }

        [Display(Name = "Car List")]
        public ICollection<Vehicle>? vehicles { get; set; }
        public ICollection<Employee>? employers { get; set; }
        public ICollection<Manager>? managers { get; set; }
    }
}
