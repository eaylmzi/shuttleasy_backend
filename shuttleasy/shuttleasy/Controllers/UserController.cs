using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.Models.dto.Login.dto;
using shuttleasy.Services;

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
        public IActionResult ValidateOTP([FromBody] EmailOtpDto emailOtpDto)
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
        public IActionResult ResetPassword([FromBody] EmailPasswordDto emailPasswordDto)
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
    }
}
