using Asp.Versioning;
using CitiesManager.Core.DTO;
using CitiesManager.Infrastructure.Identity.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.Web.Controllers.v1
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                string errorMsg = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMsg);
            }
            ApplicationUser user = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PersonName = registerDTO.PersonName,
            };
            IdentityResult identityResult = await _userManager.CreateAsync(user, registerDTO.Password);
            if(identityResult.Succeeded)
            {
                // Optionally sign in the user after registration
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(user);
            }
            else
            {
                string errorMsg = string.Join(" | ", identityResult.Errors.Select(e => e.Description));
                return Problem(errorMsg);
            }
        }

        [HttpGet("IsEmailInUse")]
        public async Task<bool> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }
    }
}
