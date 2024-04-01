namespace Factories.WebApi.BLL.Configuration
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        public required string SecretKey { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required int TokenExpiryInMinutes { get; set; }
    }
}
