using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using ModelProject;
using ModelProject.CallingAPIs;
using ModelProject.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace ApiProject
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private SignInManager<IdentityUser> _signManager;
        private Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        public IConfiguration configuration;
        ModelProject.Repo.Repository _Repository { get; set; }

        public UsersController(Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signManager = signInManager;
            this.configuration = configuration;
            this._Repository = new ModelProject.Repo.Repository();
        }

        [HttpPost("Test")]
        public async Task<IActionResult> testapi()
        {
            Thread.Sleep(2000);
            return StatusCode(201);

        }

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser(IdentityUser userForRegistration)
        {
            try
            {
                if (userForRegistration == null || !ModelState.IsValid)
                    return BadRequest();

                string password = userForRegistration.PasswordHash;
                userForRegistration.PasswordHash = null;


                var result = await _userManager.CreateAsync(userForRegistration, password);

                if (!result.Succeeded)
                {
                    return BadRequest();
                }
                else
                {
                    //var token2 = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var result2 = await _userManager.ConfirmEmailAsync(user, token2);
                    var appUser = _userManager.FindByNameAsync(userForRegistration.UserName);
                    var token = GenerateJWTToken(appUser);
                    return Ok(new
                    {
                        Token = token
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return StatusCode(201);
        }


        [HttpPost("LoginApi")]
        public async Task<IActionResult> LoginUser(IdentityUser userForRegistration)
        {
            try
            {
                if (userForRegistration == null || !ModelState.IsValid)
                    return BadRequest();

                var result = await _signManager.PasswordSignInAsync(userForRegistration.UserName,userForRegistration.PasswordHash, true,lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    //var token2 = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var result2 = await _userManager.ConfirmEmailAsync(user, token2);
                    var appUser = _userManager.FindByNameAsync(userForRegistration.UserName);
                    var token = GenerateJWTToken(appUser);
                    return Ok(new
                    {
                        Token = token
                    });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        public string GenerateJWTToken(Task<IdentityUser> userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("jwt:key").Value));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Result.UserName),
                new Claim("fullName", userInfo.Result.Id.ToString()),
                new Claim(ClaimTypes.Role, JWTPolicies.Admin),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var token = new JwtSecurityToken(
                issuer: configuration.GetSection("jwt:Issuer").Value,
                audience: configuration.GetSection("jwt:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("apiGetProducts")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetProducts()
        {
            return Ok(await _Repository.GetProducts());
        }
    }

    public class JWTPolicies
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build();
        }
        public static AuthorizationPolicy UserPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(User).Build();
        }
    }
}
