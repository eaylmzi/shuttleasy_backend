using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.Driver;
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
        private readonly IDriverLogic _driverLogic;
        public DriverController(IUserService userService , IPassengerLogic passengerLogic,IDriverLogic driverLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
        }
        [HttpPost]
        public ActionResult<Passenger> Login(string email, string password)
        {
            PasswordEncryption passwordEncryption = new PasswordEncryption();
            try
            {
                bool isLogin = _userService.LoginDriver(email, password);
                if (isLogin)
                {
                    CompanyWorker companyWorker = _driverLogic.GetCompanyWorkerWithEmail(email);
                    return Ok(companyWorker);
                }
                return BadRequest("Requirements not valid");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
