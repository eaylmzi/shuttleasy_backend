using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Models;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.Passengers.dto;
using System.Linq;
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
        public ActionResult<bool> AddPassenger(PassengerRegisterDto passengerRegisterDto)
        {
            Passenger newPassenger = new Passenger();
            

            if (!string.IsNullOrEmpty(passengerRegisterDto.Password))
            {
                ValidationResponse validationResponse = ValidatePassword(passengerRegisterDto.Password);
                if (validationResponse.Successful)
                {
                    CreatePasswordHash(passengerRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    newPassenger = _mapper.Map<Passenger>(passengerRegisterDto);
                    newPassenger.QrString = Guid.NewGuid();
                    newPassenger.PasswordHash = passwordHash;
                    newPassenger.PasswordSalt = passwordSalt;
                    //string bitString = BitConverter.ToString(passwordHash);
                    //newPassenger.Password = bitString;
                    bool result = _passengerLogic.Add(newPassenger);
                    return Ok(result);

                }
                else
                {
                    return BadRequest(validationResponse.Information);
                }               
            }
            else
            {
                return BadRequest("Password requirements is not provided");
            }
            

        }


        [HttpPost]
        public ActionResult<Passenger> GetPassenger(string id)
        {
            return _passengerLogic.Get(id);
        }

        [HttpPost]
        public ActionResult<Passenger> Login(string email,string password)
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
        private ValidationResponse ValidatePassword(string password)
        {
            ValidationResponse validationResponse;

            if (password.Length < 8 || password.Length > 15) //Min max password
            {
                validationResponse = new ValidationResponse();
                validationResponse.Successful = false;
                validationResponse.Information = "Password lenght must be between 8 and 15";
                return validationResponse;
            }
            if (!(password.Any(char.IsUpper))) //One upper case requirement
            {
                validationResponse = new ValidationResponse();
                validationResponse.Successful = false;
                validationResponse.Information = "Password must include one upper case";
                return validationResponse;
            }
            if (!(password.Any(char.IsLower))) //One lower case requirement
            {
                validationResponse = new ValidationResponse();
                validationResponse.Successful = false;
                validationResponse.Information = "Password must include one lower case";
                return validationResponse;
            }
            if (password.Contains(" ")) //Check white space
            {
                validationResponse = new ValidationResponse();
                validationResponse.Successful = false;
                validationResponse.Information = "Password must not contain gap";
                return validationResponse;
            }

            string specialChars = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
            char[] specialCharsArray = specialChars.ToCharArray();
            
            if (password.IndexOfAny(specialCharsArray)!=-1) // IndexOfAny returns -1 when the password does not include character
            {
                validationResponse = new ValidationResponse();
                validationResponse.Successful = false;
                validationResponse.Information = "Password must not include special characters";
                return validationResponse;
            }

            validationResponse = new ValidationResponse();
            validationResponse.Successful = true;
            validationResponse.Information = "Password is valid";
            return validationResponse;
        }

       


    }
}
public class ValidationResponse
{
    public bool Successful { get; set; }
    public string Information { get; set; } = null!;
}
