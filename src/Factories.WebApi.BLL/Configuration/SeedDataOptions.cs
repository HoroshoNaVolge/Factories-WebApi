namespace Factories.WebApi.BLL.Configuration
{
    public class SeedDataOptions
    {
        public const string SectionName = "SeedDataUsersDb";

        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
