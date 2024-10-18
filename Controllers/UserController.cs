
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Learning.Models;
using Learning.UserContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace Learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Context context;

        private readonly IConfiguration configuration;

        public UserController(Context context1, IConfiguration configuration1)
        {
            context = context1;   
            configuration = configuration1;
        
        }

        [HttpPost]
        [Route("AddUser")]

        public IActionResult AddUser(Users user)
        {
             var addUser =context.users.Add(user);
            context.SaveChanges();

            return Ok(AddUser);
        }

        [HttpGet]
        [Route("GetUsers")]
        public IActionResult GetUsers()
        {

           var Allusers= context.users.ToList();

            return Ok(Allusers);
        }
        [Authorize]
        [HttpGet]
        [Route("GetUser")]
        public IActionResult GetUser(int id)
        {

            var user = context.users.Find(id);

            return Ok(user);
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginUser(LoginUser loginUser)
        {
          
            var user = await context.users.FirstOrDefaultAsync(x => x.UserName == loginUser.UserName && x.Password == loginUser.Password);

            if (user != null)  
            {
                
                var userId = user.Id.ToString() ?? "UnknownId";
                var userName = user.UserName ?? "UnknownUser";
                var password = user.Password ?? "UnknownPassword";

                
                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"] ?? "UnknownSubject"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("UserName", userName),
            new Claim("Password", password)
        };

                
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

               
                var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    audience: configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(60),  
                    signingCredentials: signIn
                );


                string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

         
                return Ok(new { Token = tokenValue, User = user });
            }

      
            return Unauthorized("Invalid username or password.");
        }





    }
}