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
                bool isCreated = _userService.CheckEmail(passengerRegisterDto.Email);
                if (!isCreated)
                {
                    Passenger? newPassenger = _userService.SignUp(passengerRegisterDto, Roles.Passenger);
                    if (newPassenger != null)
                    {
                        PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(newPassenger);
                        return Ok(newPassenger);

                    }
                    return BadRequest("Not Added");

                }
                return BadRequest("Registered with this email");
            
               
            }  
            catch (Exception ex) { 
                return BadRequest(ex.Message) ; 
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
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passengerFromRequestToken = GetUserFromRequestToken();
                    Passenger? passengerFromEmail = _passengerLogic.GetPassengerWithEmail(emailPasswordDto.Email);

                    if (passengerFromEmail != null && passengerFromRequestToken != null)
                    {
                        if (passengerFromRequestToken.Id == passengerFromEmail.Id)
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
                        return BadRequest("The user and the person who sent the request are not the same");
                    }
                    return BadRequest("The user is null");

                }
                return BadRequest("Mistake about token");   



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

        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public ActionResult<PassengerInfoDto> UpdatePassenger([FromBody] UserProfileDto userProfileDto)
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passengerFromRequestToken = GetUserFromRequestToken();
                    if (passengerFromRequestToken != null)
                    {
                        Passenger? updatedPassenger = _userService.UpdatePassengerProfile(passengerFromRequestToken, userProfileDto);
                        if (updatedPassenger != null)
                        {
                            PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(updatedPassenger);
                            return Ok(passengerInfoDto);
                        }
                        return BadRequest("User not updated");
                    }
                    return BadRequest("Mistake about Token");

                }

                return BadRequest("Mistake about token");
                    


            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin}")]
        public ActionResult<PassengerInfoDto> GetPassenger([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passenger = _passengerLogic.GetPassengerWithId(idDto.Id);
                    if (passenger != null)
                    {
                        PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(passenger);
                        return Ok(passengerInfoDto);
                    }
                    return BadRequest("Passenger not found");
                }
                return BadRequest("Mistake about token");                            
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<List<Passenger>> GetAllPassengers()
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _passengerLogic.GetAllPassengers();
                    if (list != null)
                    {
                        return list;
                    }
                    return BadRequest("There is no passenger in list");
                }
                return BadRequest("The admin that send request not found");

            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }

            catch (Exception)
            {
                return StatusCode(500);
            }
        }


        private int GetUserIdFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
            string user = jwt.Claims.First(c => c.Type == "id").Value;
            int userId = int.Parse(user);
            return userId;
        }
        private string GetUserRoleFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
            string userEmail = jwt.Claims.First(c => c.Type == "role").Value;
            return userEmail;
        }

        private string GetUserTokenFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            return requestToken;
        }
        private UserVerifyingDto GetUserInformation()
        {
            UserVerifyingDto userVerifyingDto = new UserVerifyingDto();
            userVerifyingDto.Id = GetUserIdFromRequestToken();
            userVerifyingDto.Token = GetUserTokenFromRequestToken();
            userVerifyingDto.Role = GetUserRoleFromRequestToken();
            return userVerifyingDto;
        }
        private string GetUserEmailFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
            string userEmail = jwt.Claims.First(c => c.Type == "Role").Value;
            return userEmail;
        }
        private Passenger? GetUserFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            Passenger? passengerFromToken = _passengerLogic.GetPassengerWithToken(requestToken);
            return passengerFromToken;
        }



        











        /*private bool IsSamePerson(string email)
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

        }*/


    }





    
}
