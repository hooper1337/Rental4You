using Microsoft.AspNetCore.Identity;

namespace Rental4You.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string firstName { set; get; }
        public string lastName { set; get; }
        public DateTime bornDate { set; get; }
        public int nif { set; get; }
    }
}
