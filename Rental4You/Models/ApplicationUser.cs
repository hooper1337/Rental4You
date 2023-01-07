using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First Name", Prompt = "Insert first name...")]
        public string firstName { set; get; }
        [Display(Name = "Last Name", Prompt = "Insert last name...")]
        public string lastName { set; get; }
        public DateTime bornDate { set; get; }
        [Display(Name = "NIF", Prompt = "Insert nif...")]
        public int nif { set; get; }
        [Display(Name = "Available")]
        public Boolean available { get; set; }
        public DateTime? registerDate { get; set; }
    }
}
