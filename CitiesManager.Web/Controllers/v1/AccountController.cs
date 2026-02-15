using Asp.Versioning;
using CitiesManager.Core.DTO;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Infrastructure.Identity.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.Web.Controllers.v1
{
    /// <summary>
    /// Controller responsible for handling user account-related actions such as registration and login.
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        /// <summary>
        /// Constructor for AccountController, which initializes the UserManager, SignInManager, and RoleManager for handling user registration and authentication.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="roleManager"></param>
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }


        /// <summary>
        /// Checks if the provided email is already associated with an existing user account.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("IsEmailInUse")]
        public async Task<bool> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }

        /// <summary>
        /// Registers a new user with the provided registration details. 
        /// Validates the input and creates a new ApplicationUser if the registration is successful. Optionally signs in the user after registration.
        /// </summary>
        /// <param name="registerDTO"></param>
        /// <returns></returns>
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
            if (identityResult.Succeeded)
            {
                // Optionally sign in the user after registration
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Generate JWT token for the newly registered user
                UserTokenRequest userTokenRequest = new UserTokenRequest
                {
                    UserId = user.Id,
                    Email = user.Email,
                    PersonName = user.PersonName
                };
               AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(userTokenRequest);
                return Ok(authenticationResponse);
            }
            else
            {
                string errorMsg = string.Join(" | ", identityResult.Errors.Select(e => e.Description));
                return Problem(errorMsg);
            }
        }
        /// <summary>
        /// Authenticates a user with the provided login credentials. Validates the input and attempts to sign in the user using the SignInManager.
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> PostLogin(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                string errorMsg = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMsg);
            }
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user == null)
                {
                    return NoContent();
                }
                // Generate JWT token for the authenticated user after successful login
                UserTokenRequest userTokenRequest = new UserTokenRequest
                {
                    UserId = user.Id,
                    Email = user.Email,
                    PersonName = user.PersonName
                };
                AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(userTokenRequest);
                return Ok(authenticationResponse);
            }
            else
            {
                return Problem("Invalid login attempt.");
            }
        }

        /// <summary>
        /// Logs out the currently authenticated user by signing them out using the SignInManager.
        /// This will invalidate the user's authentication session.
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task<IActionResult> GetLogout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
