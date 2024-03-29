﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.Services;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using shuttleasy.DAL.Models.dto.Login.dto;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.Passengers.dto;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SuperAdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IMapper _mapper;
        public SuperAdminController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
            IMapper mapper)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
        }
        [HttpPost]
        public ActionResult<CompanyWorkerInfoDto> Login([FromBody] EmailPasswordDto emailPasswordDto)
        {
            try
            {
                CompanyWorker? companyWorker = _userService.LoginCompanyWorker(emailPasswordDto.Email, emailPasswordDto.Password);
                if (companyWorker != null)
                {
                    CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(companyWorker);
                    return Ok(driverInfoDto);
                }
                return BadRequest(Error.NotCorrectEmailAndPassword);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.SuperAdmin}")]
        public ActionResult<CompanyWorkerInfoDto> CreateAdmin([FromBody] CompanyWorkerRegisterDto adminRegisterDto)
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    bool isCreated = _userService.CheckEmailandPhoneNumberForCompanyWorker(adminRegisterDto.Email, adminRegisterDto.PhoneNumber);
                    if (!isCreated)
                    {
                        CompanyWorker? newCompanyWorker = _userService.CreateCompanyWorker(adminRegisterDto, Roles.Admin);
                        if (newCompanyWorker != null)
                        {
                            CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(newCompanyWorker);
                            return Ok(driverInfoDto);
                        }
                        return BadRequest(Error.NotAdded);
                    }
                    return BadRequest(Error.FoundEmailOrTelephone);
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
    }
}
