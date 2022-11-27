using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using shuttleasy.DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace shuttleasy.JwtToken
{
    public class JwtTokenManager : IJwtTokenManager
    {//Localde ya da serverde saklanıyormuş tokenler biz databasede saklıyoruz ?
     //bence tek seferlik 9 aylık token oluşturmak yanlış
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

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMonths(9), 
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public bool validateToken(string token, IConfiguration _configuration)
        {
            try
            {
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value ?? throw new ArgumentNullException()));
                JwtSecurityTokenHandler handler = new();
                handler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false
                } ,out SecurityToken validatedToken);
                // var jwtToken = (JwtSecurityToken)validatedToken;
                // var claims = jwtToken.Claims.ToList();
                return true;
            }
            catch(Exception)
            {
                return false;
            }


        }
       
  
    }
}
