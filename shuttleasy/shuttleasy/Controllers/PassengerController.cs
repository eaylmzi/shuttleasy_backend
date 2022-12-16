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
        public ActionResult<PassengerInfoDto> SignUp([FromBody] PassengerRegisterDto passengerRegisterDto)
        {           
            try
            {
                Passenger? newPassenger = _userService.SignUp(passengerRegisterDto,Roles.Passenger);
                if(newPassenger != null)
                {
                    PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(newPassenger);
                    return Ok(newPassenger);

                }
                return BadRequest("Not Added");
            
               
            }  
            catch (Exception ex) { //Kendi kendiliğine 401 403 atıyo ama döndürmek istersek nasıl olacak
                return BadRequest(ex.Message) ; //Bİr sıkıntı var 400 401 403 dönmek istediği zaman ne yapacam
            }
            

        }
        [HttpPost]
        public ActionResult<PassengerInfoDto> Login([FromBody] EmailPasswordDto emailPasswordDto)
        {          
            try
            {
                bool isLogin = _userService.LoginPassenger(emailPasswordDto.Email, emailPasswordDto.Password);
                if (isLogin)
                {
                    Passenger? passenger = _passengerLogic.GetPassengerWithEmail(emailPasswordDto.Email);
                    if (passenger != null)
                    {
                        PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(passenger);
                        return Ok(passengerInfoDto);
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
        public ActionResult<bool> DeletePassenger([FromBody] EmailPasswordDto emailPasswordDto)
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
                            return Ok(isDeleted);
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
        public ActionResult<PassengerInfoDto> UpdatePassenger([FromBody] UserProfileDto userProfileDto)
        {
            try
            {
                Passenger? passenger = GetPassengerFromRequestToken();
                if(passenger != null)
                {
                    Passenger? updatedPassenger = _userService.UpdatePassengerProfile(passenger, userProfileDto);
                    if (updatedPassenger != null)
                    {
                        PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(updatedPassenger);
                        return Ok(passengerInfoDto);
                    }
                    return BadRequest("User not updated");
                }
                return BadRequest("Mistake about Token");
                
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Admin},{Roles.Driver}")]
        public ActionResult<PassengerInfoDto> GetPassenger([FromBody] IdDto idDto)
        {
            try
            {
                Passenger? passenger = _passengerLogic.GetPassengerWithId(idDto.Id);
                if (passenger != null)
                {
                    PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(passenger);
                    return Ok(passengerInfoDto);
                }
                return BadRequest("Passenger not found");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        public ActionResult<string> La([FromBody] IdDto idDto)
        {
            try
            {
                Passenger passenger = _passengerLogic.GetPassengerWithId(idDto.Id);
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(passenger.Token);
                string user = jwt.Claims.First(c => c.Type == "user").Value;
                return Ok(user);
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
        private Passenger? GetPassengerFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            Passenger? passengerFromToken = _passengerLogic.GetPassengerWithToken(requestToken);
            return passengerFromToken;
        }


    }





    
}
