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
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // public DbSet<PWEB_AulasP_2223.Models.TipoDeAula> TipoDeAula { get; set; } ?
    }
}