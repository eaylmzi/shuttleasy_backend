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
using shuttleasy.Services;
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.Models.dto.Login.dto;
using shuttleasy.Models.dto.User.dto;
using System.Data;
using Microsoft.Net.Http.Headers;

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
        private readonly IUserService _userService;
        private readonly IPasswordEncryption _passwordEncryption;
        PassengerString message = new PassengerString();
        

        public PassengerController(IMapper mapper,IConfiguration configuration, IPassengerLogic passengerLogic,
            IPassengerService passengerService, IHttpContextAccessor httpContextAccessor,IMailManager mailManager,
            IJwtTokenManager jwtTokenManager,IUserService userService,IPasswordEncryption passwordEncryption)
        {

            _mapper = mapper;
            _configuration = configuration;
            _passengerLogic = passengerLogic;
            _passengerService = passengerService;
            _httpContextAccessor = httpContextAccessor;
            _mailManager = mailManager;
            _jwtTokenManager = jwtTokenManager;
            _userService = userService;
            _passwordEncryption = passwordEncryption;
        }
        //  [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]

        [HttpPost]
        public ActionResult<bool> SignUp([FromBody] PassengerRegisterDto passengerRegisterDto)
        {           
            try
            {
                Passenger newPassenger = _userService.SignUp(passengerRegisterDto,Roles.Passenger)
                        ?? throw new ArgumentNullException();
                if(newPassenger != null)
                {
                    return Ok(newPassenger);
                }
                return BadRequest("Not Added");
            
               
            }  
            catch (Exception ex) { //Kendi kendiliğine 401 403 atıyo ama döndürmek istersek nasıl olacak
                return BadRequest(ex.Message) ; //Bİr sıkıntı var 400 401 403 dönmek istediği zaman ne yapacam
            }
            

        }
        [HttpPost]
        public ActionResult<Passenger> Login([FromBody] EmailPasswordDto emailPasswordDto)
        {          
            try
            {
                bool isLogin = _userService.LoginPassenger(emailPasswordDto.Email, emailPasswordDto.Password);
                if (isLogin)
                {
                    Passenger? passenger = _passengerLogic.GetPassengerWithEmail(emailPasswordDto.Email);
                    if (passenger != null)
                    {
                        return Ok(passenger);
                    }
                    return BadRequest("The passenger not found in list");
                }
                return BadRequest("Email and password not correct");

            }         
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public ActionResult<Passenger> DeletePassenger([FromBody] EmailPasswordDto emailPasswordDto)
        {
            try
            {
                bool isSamePerson = IsSamePerson(emailPasswordDto.Email);
              
                if (isSamePerson)
                {
                    Passenger passenger = _passengerLogic.GetPassengerWithEmail(emailPasswordDto.Email)
                        ?? throw new ArgumentNullException();

                    if (_passwordEncryption.VerifyPasswordHash(passenger.PasswordHash, passenger.PasswordSalt, emailPasswordDto.Password))
                    {
                        bool isDeleted = _passengerLogic.DeletePassenger(emailPasswordDto.Email);
                        if (isDeleted)
                        {
                            return Ok();
                        }
                        else
                        {
                            return BadRequest("The user not deleted");
                        }
                    }
                    return BadRequest("The password not verified");
                }
                return BadRequest("The user and the person who sent the request are not the same"); //Neyi dönceğimi daha bilmiyom status code olarak


            }
            catch(AuthenticationException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Admin}")]
        public ActionResult<Passenger> UpdatePassenger(UserProfileDto userProfileDto)
        {
            try
            {
                string token = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
                Passenger passenger = _passengerLogic.GetPassengerWithToken(token)
                    ?? throw new AuthenticationException();
                Passenger? updatedPassenger = _userService.UpdatePassengerProfile(passenger,userProfileDto);
                if (updatedPassenger != null)
                {
                    return Ok(updatedPassenger);
                }
                return BadRequest("User not updated");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        









        [HttpPost]
        public IActionResult SendOTPEmail([FromBody] string email)
        {
            try
            {
                ResetPassword? res = _userService.SendOTP(email);
                if(res != null)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest("No attempt can be made before 180 seconds");
                }
                
            }
            catch(Exception ex) {
                return BadRequest(ex.Message);
            }
               
        }
        [HttpPost]
        public IActionResult ValidateOTP([FromBody]EmailOtpDto emailOtpDto)
        {
            try {
                EmailTokenDto? emailTokenDto = _userService.ValidateOTP(emailOtpDto.Email, emailOtpDto.Otp);
                if (emailTokenDto != null)
                {
                    return Ok(emailTokenDto);
                }
                return BadRequest("Not valid password");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult ResetPassword(EmailPasswordDto emailPasswordDto)
        {
            try {
                object? user = _userService.resetPassword(emailPasswordDto.Email, emailPasswordDto.Password);
                if (user != null)
                {
                    return Ok(user);
                }
                return BadRequest("The password has not been updated");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool IsSamePerson(string email)
        {
            string token = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            Passenger passengerFromToken = _passengerLogic.GetPassengerWithToken(token)
                ?? throw new AuthenticationException();
            Passenger passenger = _passengerLogic.GetPassengerWithEmail(email)
                ?? throw new ArgumentNullException();

            if (passengerFromToken.Id == passenger.Id)
            {
                return true;
            }
            return false;

        }


    }





    
}
