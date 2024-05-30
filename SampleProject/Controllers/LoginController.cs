using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SampleProject.Models.ProductInventory;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SampleProject.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration configuration)
        {
            _config = configuration;
        }

        private Login Authentication(Login login)
        {
            Login _user = null;
            if (login.UserName != null && login.Password != null)
            {
                _user = login;
            }

            return _user;
        }

        private string GenerateToken(Login login)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"], _config["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]Login login) { 
            IActionResult res = Unauthorized();
            var user_ = Authentication(login);
            if (user_ != null) { 
                var token = GenerateToken(user_);
                res =  Ok(new { token = token });
            }

            return res;
        }

    }
}
