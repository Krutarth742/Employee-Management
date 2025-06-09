using Employee_Management_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Employee_Management_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider = "Google", string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpPost("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback([FromBody] TokenRequest request)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(request.IdToken);
            var email = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;
            var name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var sub = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sub))
            {
                return BadRequest("Invalid token.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Name = name ?? email.Split('@')[0],
                    Phone = "0000000000",
                    Age = 18,
                    Salary = 0,
                    DepartmentID = 1
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return BadRequest("Failed to create user.");
            }

            var jwt = await GenerateJwtToken(user);
            return Ok(new { token = jwt });
        }

        [HttpGet("facebook-login")]
        public IActionResult FacebookLogin(string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(FacebookTokenLogin), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return Challenge(properties, "Facebook");
        }

        [HttpPost("facebook-token")]
        public async Task<IActionResult> FacebookTokenLogin([FromBody] TokenRequest request)
        {

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(request.IdToken) as JwtSecurityToken;

                var email = jsonToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var name = jsonToken?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                var userId = jsonToken?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;


                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email not found in token claims.");
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        Name = name ?? email.Split('@')[0],
                        Phone = "0000000000",
                        Age = 18,
                        Salary = 0,
                        DepartmentID = 1
                    };
                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return BadRequest("User creation failed.");
                }

                var jwt = await GenerateJwtToken(user);
                return Ok(new { token = jwt });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication error: {ex.Message}");
                return BadRequest($"Authentication error: {ex.Message}");
            }
        }


        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
