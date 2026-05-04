using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TutoringAcademy.Settings;
using TutoringAcademy.Models;

namespace TutoringAcademy.Services
{
    // This service is responsible for generating JSON Web Tokens (JWT) for authenticated users. It uses the JWT settings defined in the application configuration to create a token that includes user information and claims.
    public class JWTService(IOptions<JWTSettings> jwtSettings)
    {
        // The JWT settings are injected into the service using IOptions, allowing the service to access the secret key, issuer, audience, and expiry time for token generation.
        private readonly JWTSettings _jwtSettings = jwtSettings.Value;

        // This method generates a JWT token for a given user. It creates a token descriptor that includes the user's claims, expiration time, issuer, audience, and signing credentials. The generated token is then returned as a string.
        public string GenerateToken(User user)
        {
            // Create a token handler to generate the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            // Convert the secret key from the JWT settings into a byte array
            var key = System.Text.Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            // Define the token descriptor with user claims, expiration, issuer, audience, and signing credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // The subject of the token includes claims such as user ID, username, email, and role, which can be used for authorization and authentication purposes in the application.
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                ]),
                // The token is set to expire after a certain number of minutes defined in the JWT settings, ensuring that the token is valid only for a limited time and enhancing security.
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                // The issuer and audience of the token are set based on the values defined in the JWT settings, which can be used to validate the token when it is received by the server.
                Issuer = _jwtSettings.Issuer,
                // The signing credentials are created using the secret key and the HMAC SHA256 algorithm, which ensures that the token is securely signed and can be verified by the server when it is received.
                Audience = _jwtSettings.Audience,
                // The signing credentials are created using the secret key and the HMAC SHA256 algorithm, which ensures that the token is securely signed and can be verified by the server when it is received.
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            // The token is created using the token handler and the defined token descriptor, and then it is written to a string format to be returned to the client.
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // The generated token is returned as a string, which can be used by the client for authentication and authorization in subsequent requests to the server.
            return tokenHandler.WriteToken(token);
        }
    }
}