using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.BLL.Database.DbConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<IdentityUser>
    {
        public void Configure(EntityTypeBuilder<IdentityUser> builder)
        {
            builder.HasData(new IdentityUser
            {
                Id = "fe342990-c53a-4bb9-89b6-4b4482e956fb",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@mail.ru",
                NormalizedEmail = "ADMIN@MAIL.RU",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAENHAMmgih8HUHvasMFLvvPqwmV/eEMdj8+d8hvvQ79SiWNGomApGcJe65AHTWwUFRQ==" //P@ssw0rd
            });
        }
    }
}
