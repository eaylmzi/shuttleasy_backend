﻿using AutoMapper;
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
using shuttleasy.DAL.Models.dto.Passengers.dto;
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
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.Login.dto;
using shuttleasy.DAL.Models.dto.User.dto;
using System.Data;
using Microsoft.Net.Http.Headers;
using shuttleasy.Resource;
using shuttleasy.LOGIC.Logics.JoinTables;
using shuttleasy.DAL.Models.dto.JoinTables.dto;

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
        private readonly IJoinTableLogic _joinTableLogic;
        PassengerString message = new PassengerString();
        List<PassengerShuttleDetailsDto> emptyList = new List<PassengerShuttleDetailsDto>();
        

        public PassengerController(IMapper mapper,IUserService userService,
            IPasswordEncryption passwordEncryption, IPassengerLogic passengerLogic, IJoinTableLogic joinTableLogic)
        {
            _passengerLogic = passengerLogic;
            _mapper = mapper;
            _userService = userService;
            _passwordEncryption = passwordEncryption;
            _joinTableLogic = joinTableLogic;
        }
        //  [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]

        [HttpPost]
        public ActionResult<PassengerInfoDto> SignUp([FromBody] PassengerRegisterDto passengerRegisterDto)
        {           
            try
            {
                bool isCreated = _userService.CheckEmailandPhoneNumberForPassengers(passengerRegisterDto.Email,passengerRegisterDto.PhoneNumber);
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
                Passenger? passenger = _userService.LoginPassenger(emailPasswordDto.Email, emailPasswordDto.Password);
                if (passenger != null)
                {
                    PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(passenger);
                    return Ok(passengerInfoDto);
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
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passengerFromRequestToken = TokenHelper.GetUserFromRequestToken(Request.Headers,_passengerLogic);
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
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passengerFromRequestToken = TokenHelper.GetUserFromRequestToken(Request.Headers,_passengerLogic);
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
        
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<PassengerInfoDto> GetPassenger([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
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
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<PassengerShuttleDetailsDto> GetMyShuttleSessions()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    int userId = TokenHelper.GetUserIdFromRequestToken(Request.Headers);
                    var list = _joinTableLogic.OzimYapmaz(userId);
                    if(list.Count != 0)
                    {
                        return Ok(list);
                    }
                    return Ok(emptyList);
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
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
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




















        /*
         * 
         *  private int GetUserIdFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
            string user = jwt.Claims.First(c => c.Type == "id").Value;
            int userId = int.Parse(user);
            return userId;
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

        }*/


    }






}
