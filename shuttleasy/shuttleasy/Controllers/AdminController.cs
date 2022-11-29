using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.Driver;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Passengers.dto;
using shuttleasy.Services;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly IDriverLogic _driverLogic;
        public AdminController(IUserService userService, IPassengerLogic passengerLogic ,IDriverLogic driverLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
        }
        [HttpPost]
        public ActionResult<bool> SignUpDriver(DriverRegisterDto driverRegisterDto)
        {
            try
            {
                CompanyWorker newCompanyWorker = _userService.SignUp(driverRegisterDto, Roles.Driver);
                if (newCompanyWorker != null)
                {
                    return Ok(newCompanyWorker);
                }
                return BadRequest("Not Added");


            }
            catch (Exception ex)
            { //Kendi kendiliğine 401 403 atıyo ama döndürmek istersek nasıl olacak
                return BadRequest(ex.Message); //Bİr sıkıntı var 400 401 403 dönmek istediği zaman ne yapacam
            }


        }
       

       
    }
}
