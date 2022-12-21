namespace Rental4You.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }

}
