using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.BLL.Database.DbConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole { Id = "70a2bf46-7e0a-4559-a625-5b3ca954b3cf", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "66f7058e-10a9-42f7-8bf1-62e5e117e49c", Name = "User", NormalizedName = "USER" });
        }
    }
}
