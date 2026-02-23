using CitiesManager.Core.DTO;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CitiesManager.Infrastructure.AuthenticationService
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Generates a JWT token based on the provided user token request.
        /// The token includes claims such as user ID, email, and person name, and is signed using a secret key from the configuration.
        /// The token also has an expiration time defined in the configuration.
        /// </summary>
        /// <param name="userTokenRequest"></param>
        /// <returns></returns>
        public AuthenticationResponse CreateJwtToken(UserTokenRequest userTokenRequest)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MIN"]));
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userTokenRequest.UserId.ToString()), // Subject claim with user ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID for uniqueness
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Issued at claim as Unix timestamp

                new Claim(ClaimTypes.NameIdentifier, userTokenRequest.Email.ToString()), // Unique identifier for the user (email)
                new Claim(ClaimTypes.Name, userTokenRequest.PersonName ?? string.Empty),
                new Claim(ClaimTypes.Email, userTokenRequest.Email ?? string.Empty)
            };

            // Create a symmetric security key using the secret key from configuration
            SymmetricSecurityKey key = new 
                SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:SECRET_KEY"]!));

            // Create signing credentials using the security key and HMAC-SHA256 algorithm
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:ISSUER"],
                audience: _configuration["Jwt:AUDIENCE"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            // Create a JwtSecurityTokenHandler to write the token to a string
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string finalToken = tokenHandler.WriteToken(token);

            // Return the authentication response with the token and expiration
            return new AuthenticationResponse
            {
                PersonName = userTokenRequest.PersonName,
                Email = userTokenRequest.Email,
                Token = finalToken,
                Expiration = expiration,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDateTime = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["RefreshToken:EXPIRATION_MIN"]!))
            };
        }

        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? jwtToken)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:AUDIENCE"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:ISSUER"],
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:SECRET_KEY"]!)),
                ValidateLifetime = false // Jwt token may be expired, but we want to get the claims from it for refresh token validation
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return claimsPrincipal;
        }

        /// <summary>
        /// Generates a cryptographically secure random refresh token encoded as a Base64 string.
        /// </summary>
        /// <remarks>The generated token is suitable for use in authentication scenarios where a secure,
        /// unpredictable value is required. Each call produces a unique token. The caller is responsible for securely
        /// storing and managing the token.</remarks>
        /// <returns>A Base64-encoded string representing a randomly generated refresh token.</returns>
        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var randomNumberGenerator = RandomNumberGenerator
                .Create();
            randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
