namespace Rental4You.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string brand { set; get; }
        public string model { set; get; }
        public string type { set; get; }
        public string place { set; get; }
        public DateTime withdraw { set; get; }
        public DateTime deliver { set; get; }
        public int costPerDay { set; get; }
        public Company company { set; get; }
    }

}
