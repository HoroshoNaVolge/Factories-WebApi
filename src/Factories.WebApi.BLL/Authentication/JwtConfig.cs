namespace Factories.WebApi.BLL.Authentication
{
    public sealed class JwtConfig
    {
        public const string SectionName = "Jwt";

        public required string SecretKey { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required int TokenExpiryInMinutes { get; set; }
    }
}
