using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Models;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.Passengers.dto;
using System.Security.Cryptography;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private PassengerLogic _passengerLogic = new shuttleasy.LOGIC.Logics.PassengerLogic();
        private readonly IMapper _mapper;

        public PassengerController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> AddPassenger(PassengerRegisterDto passengerRegisterDto)
        {
            Passenger newPassenger = new Passenger();

            if (passengerRegisterDto.Password != null )
            {
                CreatePasswordHash(passengerRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                newPassenger = _mapper.Map<Passenger>(passengerRegisterDto);
                newPassenger.QrString = Guid.NewGuid();
                newPassenger.PasswordHash = passwordHash;
                newPassenger.PasswordSalt = passwordSalt;
                //string bitString = BitConverter.ToString(passwordHash);
                //newPassenger.Password = bitString;            
            }
            bool result = _passengerLogic.Add(newPassenger);
            return Ok(result);
        }


        [HttpPost]
        public ActionResult<Passenger> GetPassenger(string id)
        {
            return _passengerLogic.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<Passenger>> Login(string email,string password)
        {
           Passenger passenger = _passengerLogic.GetPassengerWithEmail(email);
           if(string.IsNullOrEmpty(passenger.Email))
            {
                return BadRequest("Email is not in the list");
            }
            bool isMatched = VerifyPasswordHash(passenger, password);
            if (isMatched)
            {
                return Ok("Login is successfull");
            }
            else
            {
                return BadRequest("Password is not matched");
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
      
        
    }
}
