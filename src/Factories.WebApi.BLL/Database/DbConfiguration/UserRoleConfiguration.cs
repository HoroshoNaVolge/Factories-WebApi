using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.BLL.Database.DbConfiguration
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(new IdentityUserRole<string>
            {
                UserId = "fe342990-c53a-4bb9-89b6-4b4482e956fb",
                RoleId = "70a2bf46-7e0a-4559-a625-5b3ca954b3cf"
            });
        }
    }
}
