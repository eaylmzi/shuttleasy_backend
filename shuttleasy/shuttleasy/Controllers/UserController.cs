﻿using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.Models.dto.Login.dto;
using shuttleasy.Services;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        public UserController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
        }
        [HttpPost]
        public ActionResult<ResetPassword> SendOTPEmail([FromBody] EmailDto emailDto)
        {
            try
            {
                ResetPassword? res = _userService.SendOTP(emailDto.Email);
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
        public ActionResult<EmailTokenDto> ValidateOTP([FromBody] EmailOtpDto emailOtpDto)
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
        public ActionResult<object> ResetPassword([FromBody] EmailPasswordDto emailPasswordDto)
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
