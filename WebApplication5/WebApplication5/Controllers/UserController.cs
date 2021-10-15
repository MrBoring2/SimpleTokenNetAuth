using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication5.Controllers
{

    public class UserController : ControllerBase
    {
        // GET: api/<UserController>
        private TestUsersContext _context;
        public UserController(TestUsersContext context)
        {
            _context = context;
        }
        [HttpPost("/token")]
        public async Task<IActionResult> Token(string Login, string Password)
        {
            var identity = await GetIdentity(Login, Password);
            if (identity == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
                role = identity.FindFirst(ClaimTypes.Role).Value,
                name_identifier = identity.FindFirst(ClaimTypes.NameIdentifier).Value
            };
            return Ok(response);
        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            User person = await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(x => x.Login == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, person.Login),
                    new Claim(ClaimTypes.Name, person.Name),
                    new Claim(ClaimTypes.Role, person.Role.Name)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimTypes.Name,
                    ClaimTypes.Role);
                return claimsIdentity;
            }
            // если пользователь не найден
            return null;
        }
    }
}
