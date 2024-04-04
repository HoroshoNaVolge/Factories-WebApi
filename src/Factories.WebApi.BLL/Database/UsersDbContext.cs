using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.BLL.Database
{
    public class UsersDbContext(DbContextOptions<UsersDbContext> options) : IdentityDbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Id = "70a2bf46-7e0a-4559-a625-5b3ca954b3cf", Name = "Admin", NormalizedName = "ADMIN" },
                                                         new IdentityRole { Id = "66f7058e-10a9-42f7-8bf1-62e5e117e49c", Name = "User", NormalizedName = "USER" });

            modelBuilder.Entity<IdentityUser>().HasData(new IdentityUser
            {
                Id = "fe342990-c53a-4bb9-89b6-4b4482e956fb",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@mail.ru",
                NormalizedEmail = "ADMIN@MAIL.RU",
                EmailConfirmed = true,
                //Соответствует P@ssw0rd
                PasswordHash = "AQAAAAIAAYagAAAAENHAMmgih8HUHvasMFLvvPqwmV/eEMdj8+d8hvvQ79SiWNGomApGcJe65AHTWwUFRQ==",
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = "fe342990-c53a-4bb9-89b6-4b4482e956fb",
                RoleId = "70a2bf46-7e0a-4559-a625-5b3ca954b3cf"
            });

            modelBuilder.Entity<IdentityUserClaim<string>>().HasData(
            new IdentityUserClaim<string>
            {
                Id = 1,
                UserId = "fe342990-c53a-4bb9-89b6-4b4482e956fb",
                ClaimType = "UnitOperator",
                ClaimValue = "true"
            },
            new IdentityUserClaim<string>
            {
                Id = 2,
                UserId = "fe342990-c53a-4bb9-89b6-4b4482e956fb",
                ClaimType = "TankOperator",
                ClaimValue = "true"
            });
        }
    }
}
