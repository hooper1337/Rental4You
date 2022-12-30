namespace Rental4You.Models
{
    public class Manager
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company company { get; set; }
        public ApplicationUser applicationUser { get; set; }
    }
}
