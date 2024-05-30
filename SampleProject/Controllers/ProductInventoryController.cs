using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SampleProject.Data;
using SampleProject.Models.ProductInventory;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SampleProject.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ProductInventoryController : ControllerBase
    {
        private readonly ProductInventoryContext _db;
        private IConfiguration _config;

        public ProductInventoryController(IConfiguration configuration, ProductInventoryContext db) { 
            _db = db;
            _config = configuration;
        }

        private User Authentication(Login login)
        {
            User _user = null;
            if (login.UserName != null && login.Password != null)
            {
                var u = _db.Logins.FirstOrDefault(x => x.Password == login.Password);
                if (u != null) { 
                    _user = _db.Users.FirstOrDefault(us => us.Id == u.Id);
                }
            }

            return _user;
        }

        private string GenerateToken()
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
        [HttpPost("user/login")]
        public IActionResult UserLogin([FromBody] Login login)
        {
            IActionResult res = Unauthorized();
            var user_ = Authentication(login);
            if (user_ != null)
            {
                var token = GenerateToken();
                res = Ok(new {name = user_.Name, token = token });
            }

            return res;
        }

        

    }
}
