using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Manager
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company company { get; set; }

        [Display(Name = "Available")]
        public Boolean available { get; set; }
        public ApplicationUser applicationUser { get; set; }
    }
}
