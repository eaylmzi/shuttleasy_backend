using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Passengers.dto;
using shuttleasy.Services;
using System.Data;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        public AdminController(IUserService userService, IPassengerLogic passengerLogic ,ICompanyWorkerLogic driverLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
        }
        [HttpPost]
        public ActionResult<string> Login(string email, string password)
        {
            try
            {
                bool isLogin = _userService.LoginCompanyWorker(email, password);
                if (isLogin)
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithEmail(email);
                    if (companyWorker != null)
                    {
                        return Ok(companyWorker.Token);
                    }
                    return BadRequest("The admin not found in list");
                    
                }
                return BadRequest("Email and password not correct");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult<CompanyWorker> CreateDriver(DriverRegisterDto driverRegisterDto)
        {
            try
            {
                CompanyWorker newCompanyWorker = _userService.CreateDriver(driverRegisterDto, Roles.Driver);
                if (newCompanyWorker != null)
                {
                    return Ok(newCompanyWorker);
                }
                return BadRequest("Not Added");
            }
            catch (Exception ex)
            { 
                return BadRequest(ex.Message); 
            }


        }
        [HttpPost]
        public ActionResult<CompanyWorker> CreatePassenger(PassengerRegisterPanelDto passengerRegisterPanelDto)
        {
            try
            {
                Passenger newPassenger = _userService.CreatePassenger(passengerRegisterPanelDto, Roles.Passenger)
                         ?? throw new ArgumentNullException();
                if (newPassenger != null)
                {
                    return Ok(newPassenger);
                }
                return BadRequest("Not Added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<Passenger> GetPassenger(int id)
        {
            try
            {
                return _passengerLogic.GetPassengerWithId(id);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        [HttpPost]
        public ActionResult<List<Passenger>> GetAllPassengers()
        {
            try
            {
                return _passengerLogic.GetAllPassengers();
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

    }
}
