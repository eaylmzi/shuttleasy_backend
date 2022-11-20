using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using shuttleasy.DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace shuttleasy.JwtToken
{
    public class JwtTokenManager : IJwtTokenManager
    {
        public string CreateToken(Passenger passenger, IConfiguration _configuration)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role,"Passenger"),
                new Claim(ClaimTypes.NameIdentifier,passenger.IdentityNum),
                new Claim(ClaimTypes.Name,passenger.Name)

            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
               _configuration.GetSection("AppSettings:Token").Value ?? throw new ArgumentNullException()));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMonths(9), 
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
