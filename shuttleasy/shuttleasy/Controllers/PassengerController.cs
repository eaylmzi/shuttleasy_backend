using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.Passengers.dto;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private PassengerLogic _passengerLogic = new shuttleasy.LOGIC.Logics.PassengerLogic();
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        PassengerString message = new PassengerString();

        public PassengerController(IMapper mapper,IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost]
        public ActionResult<Passenger> GetPassenger(string id)
        {
            try
            {
                return _passengerLogic.GetPassengerWithId(id);
            }

            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public ActionResult<bool> AddPassenger(PassengerRegisterDto passengerRegisterDto)
        {
            Passenger newPassenger = new Passenger(); 
            try
            {
                if (!string.IsNullOrEmpty(passengerRegisterDto.Password))
                {
                    CreatePasswordHash(passengerRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    newPassenger = _mapper.Map<Passenger>(passengerRegisterDto);
                    newPassenger.QrString = Guid.NewGuid();
                    newPassenger.PasswordHash = passwordHash;
                    newPassenger.PasswordSalt = passwordSalt;
                    newPassenger.IsPayment = false;
                    newPassenger.Verified = false;
                    //string bitString = BitConverter.ToString(passwordHash);
                    //newPassenger.Password = bitString;
                    bool result = _passengerLogic.Add(newPassenger);
                    return Ok(result);

                }

                else
                {
                 
                    return BadRequest(message.passwordNull);

                }
            }
            catch (Exception) { 
                return StatusCode(500) ;
            }
            

        }

        [HttpPost]
        public ActionResult<Passenger> Login(string email,string password)
        {
            Passenger passenger = _passengerLogic.GetPassengerWithEmail(email);

            bool isMatched = VerifyPasswordHash(passenger, password);
            if (isMatched)
            {
                string token = CreateToken(passenger);
                return Ok(token);
            }
            else
            {
                return BadRequest(message.loginUnsuccesful);
            }
           
        }


        private void CreatePasswordHash(string password,out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;               
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                
            }               
        }
        private bool VerifyPasswordHash(Passenger passenger,string password)
        {
            using (var hmac = new HMACSHA512(passenger.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                bool isMatched = computedHash.SequenceEqual(passenger.PasswordHash);
                return isMatched;
            }
        }       

        private string CreateToken(Passenger passenger)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,passenger.Name)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
               _configuration.GetSection("AppSettings:Token").Value ));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                ) ;
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);



            return jwt;
        }
       


    }
}
