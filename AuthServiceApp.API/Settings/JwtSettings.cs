namespace AuthServiceApp.API.Settings
{
    public class JwtSettings
    {
        public const string SectionName = "Jwt";
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string Key { get; set; }
        public required int AccessTokenValidityMinutes { get; set; }
        public required int RefreshTokenValidityDays { get; set; }
    }
}
