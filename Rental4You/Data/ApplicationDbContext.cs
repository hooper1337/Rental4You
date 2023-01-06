using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rental4You.Models;

namespace Rental4You.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Vehicle> vehicles { set; get; }
        public DbSet<Company> companies { set; get; }
        public DbSet<Reservation> reservations { get; set; }
        public DbSet<Employee> employees { set; get; }
        public DbSet<Manager> managers { set; get; }
        public DbSet<Category> categories { set; get; }
        public DbSet<VehicleState> vehicleStates { set; get; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}