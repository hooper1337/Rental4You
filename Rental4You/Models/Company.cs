namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string name { get; set; }
        public int classification { get; set; } // from 0 to 10
        public ICollection<Vehicle> vehicles { get; set; }
    }
}
