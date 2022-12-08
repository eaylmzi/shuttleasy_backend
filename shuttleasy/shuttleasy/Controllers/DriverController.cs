

using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.Models.dto.Login.dto;
using shuttleasy.Models.dto.Passengers.dto;
using shuttleasy.Services;

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
    }
}
