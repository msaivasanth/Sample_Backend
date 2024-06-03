using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SampleProject.Data;
using SampleProject.Models.DTO;
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
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("user/me")]
        public IActionResult CheckTokenExpiry()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            

            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized(new { message = "Token is missing or invalid" });
            }

            var token = authHeader.Trim();

            var signingKey = _config["Jwt:Key"];
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    if (jwtSecurityToken.ValidTo < DateTime.UtcNow)
                    {
                        return Unauthorized(new { message = "Token is expired" });
                    }

                    return Ok(new { message = "Token is valid" });
                }
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Token validation failed", error = ex.Message });
            }

            return Unauthorized(new { message = "Token is invalid" });
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

        [HttpGet("products")]

        public async Task<ActionResult<List<ProductDto>>> GetProducts()
        {
            
            var products = await _db.ProductDtos.FromSqlRaw("spGetProducts").ToListAsync();
            List<ProductInfo> pros = new List<ProductInfo>();

            for(int i =  0; i < products.Count; i++)
            {
                int id = products[i].Id;
                Console.WriteLine(id);
                string[] images = _db.Images.Where(i => i.Id == id).Select(i => i.ImageUrl).ToArray();
                var pro = new ProductInfo()
                {
                    id = id,
                    title = products[i].Title,
                    description = products[i].Description,
                    price = products[i].Price,
                    rating = products[i].Rating,
                    brand = products[i].Brand_Name,
                    category = products[i].Category_Name,
                    thumbnail = products[i].Thumbnail,
                    images = images
                };
                pros.Add(pro);
            }


            return Ok(pros);    
        }


    }
}
