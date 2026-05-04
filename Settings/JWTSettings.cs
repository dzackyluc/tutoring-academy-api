namespace TutoringAcademy.Settings
{
    // This class represents the settings required for JWT (JSON Web Token) authentication in the application. It includes properties such as SecretKey, ExpiryMinutes, Issuer, and Audience, which are used to configure the generation and validation of JWT tokens for user authentication and authorization.
    public class JWTSettings
    {
        public string SecretKey { get; set; } = null!;
        public int ExpiryMinutes { get; set; }
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
    }
}