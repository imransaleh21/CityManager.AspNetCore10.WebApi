using CitiesManager.Core.DTO;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;

namespace CitiesManager.Infrastructure.AuthenticationService
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AuthenticationResponse CreateJwtToken(UserTokenRequest userTokenRequest)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt: EXPIRATION_MIN"]));

        }
    }
}
