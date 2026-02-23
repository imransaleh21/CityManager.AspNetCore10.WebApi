using System;
using System.Collections.Generic;
using System.Text;

namespace CitiesManager.Core.DTO
{
    public class AuthenticationResponse
    {
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public DateTime RefreshTokenExpirationDateTime { get; set; }
    }
}
