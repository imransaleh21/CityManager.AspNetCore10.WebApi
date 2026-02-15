using CitiesManager.Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiesManager.Core.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse CreateJwtToken(UserTokenRequest userTokenRequest);
    }
}
