using CitiesManager.Core.DTO;
using System.Security.Claims;


namespace CitiesManager.Core.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse CreateJwtToken(UserTokenRequest userTokenRequest);
        ClaimsPrincipal? GetPrincipalFromJwtToken(string? jwtToken);
    }
}
