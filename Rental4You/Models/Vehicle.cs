using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        [Display(Name = "Brand", Prompt = "Insert vehicle brand...")]
        public string brand { set; get; }
        [Display(Name = "Model", Prompt = "Insert vehicle model...")]
        public string model { set; get; }
        [Display(Name = "Type", Prompt = "Insert vehicle type...")]
        public string type { set; get; }
        [Display(Name = "Place", Prompt = "Insert vehicle place...")]
        public string place { set; get; }
        [Display(Name = "Withdraw Date", Prompt = "Choose vechicle withdraw date...")]
        public int costPerDay { set; get; }
        public int? CompanyId { get; set; }
        [Display(Name = "Company", Prompt = "Choose vehicle company...")]
        public Company? company { set; get; }

        [Display(Name = "Available", Prompt = "Is this vehicle available...")]
        public Boolean available { set; get; }
    }

}
