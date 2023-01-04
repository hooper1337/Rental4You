using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Name", Prompt = "Insert name for the category...")]
        public string name { get; set; }

        [Display(Name = "Description", Prompt = "Insert description for the category...")]
        public string description { get; set; }
        public ICollection<Vehicle>? vehicles { get; set; }
    }
}
