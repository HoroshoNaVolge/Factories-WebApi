using Factories.WebApi.BLL.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Factories.WebApi.BLL.Database
{
    public class UsersDbInitializer
    {
        public static async Task SeedData(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<SeedDataOptions> options)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            if (await userManager.FindByNameAsync("admin") == null)
            {
                var user = new IdentityUser
                {
                    UserName = options.Value.Username,
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, options.Value.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");

                    await userManager.AddClaimAsync(user, new Claim("UnitOperator", "true"));
                    await userManager.AddClaimAsync(user, new Claim("TankOperator", "true"));
                }
            }
        }
    }
}