﻿

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.Models.dto.Login.dto;
using shuttleasy.Models.dto.Passengers.dto;
using shuttleasy.Models.dto.User.dto;
using shuttleasy.Services;
using System.Data;
using System.Security.Authentication;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DriverController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        public DriverController(IUserService userService , IPassengerLogic passengerLogic,ICompanyWorkerLogic driverLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
        }
        [HttpPost]
        public ActionResult<CompanyWorker> Login([FromBody] EmailPasswordDto emailPasswordDto)
        {
            PasswordEncryption passwordEncryption = new PasswordEncryption();
            try
            {
                bool isLogin = _userService.LoginCompanyWorker(emailPasswordDto.Email, emailPasswordDto.Password);
                if (isLogin)
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithEmail(emailPasswordDto.Email);
                    if (companyWorker != null)
                    {
                        return Ok(companyWorker);
                    }
                    return BadRequest("The driver not found in list");
                }
                return BadRequest("Email and password not correct");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin}")]
        public ActionResult<CompanyWorker> UpdateDriver(DriverProfileDto driverProfileDto)
        {
            try
            {               
                CompanyWorker? companyWorker = GetCompanyWorkerFromRequestToken();
                if(companyWorker != null)
                {
                    CompanyWorker? updatedDriver = _userService.UpdateDriverProfile(companyWorker, driverProfileDto);
                    if (updatedDriver != null)
                    {
                        return Ok(updatedDriver);
                    }
                    return BadRequest("Driver not updated");
                }
                return BadRequest("Mistake about token");
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public IActionResult SendOTPEmail(string email)
        {
            try
            {
                ResetPassword? res = _userService.SendOTP(email);
                if (res != null)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest("No attempt can be made before 180 seconds");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        public IActionResult ValidateOTP([FromBody]EmailOtpDto emailOtpDto)
        {
            try
            {
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
        public IActionResult ResetPassword([FromBody]EmailPasswordDto emailPasswordDto)
        {
            try
            {
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



        private CompanyWorker? GetCompanyWorkerFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            CompanyWorker? companyWorkerFromToken = _driverLogic.GetCompanyWorkerWithToken(requestToken);
            return companyWorkerFromToken;
        }
    }
}

