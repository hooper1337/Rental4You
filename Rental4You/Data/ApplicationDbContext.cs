using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rental4You.Models;

namespace Rental4You.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Vehicle> vehicles { set; get; }
        public DbSet<Company> companies { set; get; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
    }
}