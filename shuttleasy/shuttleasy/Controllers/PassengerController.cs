using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.JwtToken;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.Passengers.dto;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private IPassengerLogic _passengerLogic;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        PassengerString message = new PassengerString();

        public PassengerController(IMapper mapper,IConfiguration configuration, IPassengerLogic passengerLogic)
        {
            _mapper = mapper;
            _configuration = configuration;
            _passengerLogic = passengerLogic;
        }

        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public ActionResult<Passenger> GetPassenger(string id)
        {
            try
            {
                return _passengerLogic.GetPassengerWithId(id);
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        [HttpPost,Authorize(Roles = $"{Roles.Passenger},{Roles.Driver}")]
        public ActionResult<List<Passenger>> GetAllPassengers()
        {
            try
            {
                return _passengerLogic.Get();
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
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
            PasswordEncryption passwordEncryption = new PasswordEncryption();
            IJwtTokenManager jwtToken = new JwtTokenManager();
            try
            {
                newPassenger.QrString = Guid.NewGuid();
                newPassenger.IsPayment = false;
                if (!string.IsNullOrEmpty(passengerRegisterDto.Password))
                {
                    passwordEncryption.CreatePasswordHash(passengerRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    newPassenger = _mapper.Map<Passenger>(passengerRegisterDto);
                    newPassenger.PasswordHash = passwordHash;
                    newPassenger.PasswordSalt = passwordSalt;
                    newPassenger.Verified = true;

                    string token = jwtToken.CreateToken(newPassenger, _configuration);
                    newPassenger.Token = token;
                }
                else
                {
                    newPassenger.Verified = false;
                }

                bool result = _passengerLogic.Add(newPassenger);
                return Ok(result);
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                return BadRequest(ex.Message);
            }
            //catch (EncoderFallback ex)           
            catch (Exception) { 
                return StatusCode(500) ;
            }
            

        }

        [HttpPost]
        public ActionResult<Passenger> Login(string email,string password)
        {          
            PasswordEncryption passwordEncryption = new PasswordEncryption();
            JwtTokenManager jwtToken = new JwtTokenManager();
            try
            {
                Passenger passenger = _passengerLogic.GetPassengerWithEmail(email);
                bool isMatched = passwordEncryption.VerifyPasswordHash(passenger, password);
                if (isMatched)
                {
                    string token = jwtToken.CreateToken(passenger, _configuration);
                    return Ok(passenger);
                }
                else
                {
                    return BadRequest(message.loginUnsuccesful);
                }

            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(SecurityTokenEncryptionFailedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }



        }

    }
    public static class Roles
    {
        public const string Passenger = "Passenger";
        public const string Driver = "Driver";
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
    }
}
