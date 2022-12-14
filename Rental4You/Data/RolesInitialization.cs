using Microsoft.AspNetCore.Identity;
using Rental4You.Models;

namespace Rental4You.Data
{
    public enum Roles 
    {
        Client, 
        Employer, 
        Manager, 
        Admin
    }
    public static class RolesInitialization
    {
        public static async Task generateInitialData(UserManager<ApplicationUser> userManager,
                                                    RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Manager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Employer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Client.ToString()));

            var defaultUser = new ApplicationUser
            {
                UserName = "admin@localhost.com",
                Email = "admin@localhost.com",
                firstName = "Admin",
                lastName = "Local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Admin123!");
                await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
            }
        }

    }
}
