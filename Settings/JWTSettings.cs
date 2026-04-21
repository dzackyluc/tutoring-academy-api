namespace TutoringAcademy.Settings
{
    public class JWTSettings
    {
        public string SecretKey { get; set; } = null!;
        public int ExpiryMinutes { get; set; }
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
    }
}