namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string name { get; set; }
        public ICollection<Vehicle> vehicles { get; set; }
    }
}
