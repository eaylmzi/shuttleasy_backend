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
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using shuttleasy.Mail;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private IPassengerLogic _passengerLogic;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPassengerService _passengerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailManager _mailManager;
        private readonly IJwtTokenManager _jwtTokenManager;
        PassengerString message = new PassengerString();

        public PassengerController(IMapper mapper,IConfiguration configuration, IPassengerLogic passengerLogic,
            IPassengerService passengerService, IHttpContextAccessor httpContextAccessor,IMailManager mailManager,
            IJwtTokenManager jwtTokenManager)
        {
            _mapper = mapper;
            _configuration = configuration;
            _passengerLogic = passengerLogic;
            _passengerService = passengerService;
            _httpContextAccessor = httpContextAccessor;
            _mailManager = mailManager;
            _jwtTokenManager = jwtTokenManager;
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
        [HttpPost,Authorize(Roles = $"{Roles.Admin},{Roles.Driver}")]
        public ActionResult<List<Passenger>> GetAllPassengers()
        {
            try
            {
                return _passengerLogic.GetAllPassengers();
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

        [HttpGet]
        public ActionResult<string> GetPassengerToken()
        {
            var userName = _passengerService.getPassenger();
            return Ok(userName);

        }

        [HttpPost]
        public ActionResult<bool> Validate(string jwt)
        {
            return _jwtTokenManager.validateToken(jwt,_configuration);
        }

        [HttpPost]
        public ActionResult<bool> AddPassenger(PassengerRegisterDto passengerRegisterDto)
        {
            Passenger newPassenger = new Passenger();
            PasswordEncryption passwordEncryption = new PasswordEncryption();
            try
            {
                newPassenger = _mapper.Map<Passenger>(passengerRegisterDto);
                newPassenger.QrString = Guid.NewGuid();
                newPassenger.IsPayment = false;
                if (!string.IsNullOrEmpty(passengerRegisterDto.Password))
                {
                    passwordEncryption.CreatePasswordHash(passengerRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

                   
                    newPassenger.PasswordHash = passwordHash;
                    newPassenger.PasswordSalt = passwordSalt;
                    newPassenger.Verified = true;

                    string token = _jwtTokenManager.CreateToken(newPassenger, _configuration);
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
          
            catch (Exception) { //Kendi kendiliğine 401 403 atıyo ama döndürmek istersek nasıl olacak
                return StatusCode(500) ;
            }
            

        }
        [HttpPost]
        public ActionResult<Passenger> Login(string email,string password)
        {          
            PasswordEncryption passwordEncryption = new PasswordEncryption();
            try
            {
                Passenger passenger = _passengerLogic.GetPassengerWithEmail(email);
                bool isMatched = passwordEncryption.VerifyPasswordHash(passenger, password);
                if (isMatched)
                {
                    string token = _jwtTokenManager.CreateToken(passenger, _configuration);
                    return Ok(token);
                }
                else
                {
                    return BadRequest(message.loginUnsuccesful);
                }

            }
            catch (ArgumentNullException ex) // bunların hepsi yerine Exception ex yazsam aynı işlevi görür ?
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
            catch (SqlNullValueException ex)
            {
                return BadRequest(ex.Message);
            }
            
            catch (Exception)
            {
                return StatusCode(500);
            }



        }
        [HttpPost]
        public IActionResult AuthenticateUser()
        {
            var result = _mailManager.sendMail("emreyilmaz0999@hotmail.com", _configuration);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
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
