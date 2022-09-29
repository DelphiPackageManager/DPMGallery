namespace DPMGallery.Configuration.Auth
{
    public class JwtConfig
    {
        public string ValidAudience { get; set; } = "https://localhost:5002";
        public string ValidIssuer { get; set; } = "https://localhost:5002";
        public string Secret { get; set; } = "JWTRefreshTokenHIGHsecuredPasswordVVVp1OH7Xzyr";
        public int TokenValidityInMinutes { get; set; } = 1;
        public int RefreshTokenValidityInDays { get; set; } = 7;
    }
}
