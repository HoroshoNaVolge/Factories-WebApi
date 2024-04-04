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

            // Я для примера добавлю сюда seed ролей в таблицу ролей, но самого админа в OnModelCreating корректно не добавить,
            // т.к. нужно ему добавить роль и клеймы асинхронно через UserManager (в OnModelCreating не сделать асинхронно).
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin", NormalizedName = "ADMIN" },
                                                         new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "User", NormalizedName = "USER" }); 
        }
    }
}
