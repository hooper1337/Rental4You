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
        public DateTime ?withdraw { set; get; }
        [Display(Name = "Delivery Date", Prompt = "Choose vechicle delivery date...")]
        public DateTime ?deliver { set; get; }
        [Display(Name = "Cost/Day", Prompt = "Insert vehicle cost per day...")]
        public int costPerDay { set; get; }
        [Display(Name = "Company", Prompt = "Choose vehicle company...")]
        public Company ?company { set; get; }
    }

}
