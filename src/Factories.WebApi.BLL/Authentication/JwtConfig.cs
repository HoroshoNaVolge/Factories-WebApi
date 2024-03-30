namespace Factories.WebApi.BLL.Authentication
{
    public sealed class JwtConfig
    {
        public const string SectionName = "Jwt";

        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenExpiryInMinutes { get; set; }
    }
}
