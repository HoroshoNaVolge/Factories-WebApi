using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.BLL.Database
{
    public class UsersDbContext(DbContextOptions<UsersDbContext> options) : IdentityDbContext(options)
    {
    }
}
