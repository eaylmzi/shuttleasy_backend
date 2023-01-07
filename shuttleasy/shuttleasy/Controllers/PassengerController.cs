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
        private readonly IUserService _userService;
        private readonly IPasswordEncryption _passwordEncryption;
        PassengerString message = new PassengerString();
        

        public PassengerController(IMapper mapper,IUserService userService,
            IPasswordEncryption passwordEncryption, IPassengerLogic passengerLogic)
        {
            _passengerLogic = passengerLogic;
            _mapper = mapper;
            _userService = userService;
            _passwordEncryption = passwordEncryption;
        }
        //  [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]

        [HttpPost]
        public ActionResult<PassengerInfoDto> SignUp([FromBody] PassengerRegisterDto passengerRegisterDto)
        {           
            try
            {
                bool isCreated = _userService.CheckEmailandPhoneNumber(passengerRegisterDto.Email,passengerRegisterDto.PhoneNumber);
                if (!isCreated)
                {
                    Passenger? newPassenger = _userService.SignUp(passengerRegisterDto, Roles.Passenger);
                    if (newPassenger != null)
                    {
                        PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(newPassenger);
                        return Ok(newPassenger);

                    }
                    return BadRequest(Error.NotAdded);

                }
                return BadRequest(Error.FoundEmailOrTelephone);
            
               
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
                    return BadRequest(Error.NotFoundPassenger);
                }
                return BadRequest(Error.NotCorrectEmailAndPassword);

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
                                    return BadRequest(Error.NotDeletedPassenger);
                                }
                            }
                            return BadRequest(Error.NotVerifiedPassword);
                        }
                        return BadRequest(Error.ForeignRequest);
                    }
                    return BadRequest(Error.NotFoundPassenger);

                }
                return Unauthorized(Error.NotMatchedToken);



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
                        return BadRequest(Error.NotUpdatedInformation);
                    }
                    return BadRequest(Error.NotFoundPassenger);

                }

                return Unauthorized(Error.NotMatchedToken);



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
                    return BadRequest(Error.NotFoundPassenger);
                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost, Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
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
                    return BadRequest(Error.NotFoundPassenger);
                }
                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
