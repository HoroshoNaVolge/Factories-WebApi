using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.BLL.Database.DbConfiguration
{
    public class UserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder.HasData(
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
