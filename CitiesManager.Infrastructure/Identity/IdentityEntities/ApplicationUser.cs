using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiesManager.Infrastructure.Identity.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }

    }
}
