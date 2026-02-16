using CitiesManager.Core.DTO;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), // Issued at claim

                new Claim(ClaimTypes.NameIdentifier, userTokenRequest.Email.ToString()), // Unique identifier for the user (email)
                new Claim(ClaimTypes.Name, userTokenRequest.PersonName ?? string.Empty)
            };

            // Create a symmetric security key using the secret key from configuration
            SymmetricSecurityKey key = new 
                SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:SECRET_KEY"]));

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
                Expiration = expiration
            };
        }
    }
}
