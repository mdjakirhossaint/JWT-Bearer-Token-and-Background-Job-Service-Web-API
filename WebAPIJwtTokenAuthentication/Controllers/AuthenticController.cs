using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPIJwtTokenAuthentication.Models;
using WebAPIJwtTokenAuthentication.ViewModels;

namespace WebAPIJwtTokenAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthenticController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        //private readonly RoleManager<IdentityUser> roleManager;
        private readonly IConfiguration configuration;

        public AuthenticController(UserManager<IdentityUser> _userManager
            //,
            //RoleManager<IdentityUser> _roleManager
            ,
            IConfiguration _configuration
            )
        {
            userManager = _userManager;
            //roleManager = _roleManager;
            configuration = _configuration;
        }
        // Adding a Action For Register AspNet User

        [HttpPost]

        public async Task<IActionResult> RegirsterUser([FromBody] RegisterViewModel model)
        {
            // At First Check username exist or not.
            var ExistUser = await userManager.FindByNameAsync(model.Username);
            if (ExistUser != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "This Username Already Exist in our Server" });
            }

            IdentityUser user = new IdentityUser()
            {
                NormalizedUserName = model.Name,
                UserName = model.Username,

            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Is not Register successfullly" });
            }

            return Ok(new Response { Status = "Success", Message = "User has Succesfully Inserted" });
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
                var token = new JwtSecurityToken(

                    issuer: configuration["JWT:ValidIssuer"],
                    audience: configuration["JWT:ValidAudience"],
                    expires: System.DateTime.Now.AddHours(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
                );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            return Unauthorized();
        }
    }
}
