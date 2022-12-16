using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Login.dto;
using shuttleasy.Models.dto.Passengers.dto;
using shuttleasy.Services;
using System.Data;
using System.Data.SqlClient;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IMapper _mapper;
        public AdminController(IUserService userService, IPassengerLogic passengerLogic ,ICompanyWorkerLogic driverLogic,
            IMapper mapper)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
        }
        [HttpPost]
        public ActionResult<CompanyWorkerInfoDto> Login([FromBody]EmailPasswordDto emailPasswordDto)
        {
            try
            {
                bool isLogin = _userService.LoginCompanyWorker(emailPasswordDto.Email, emailPasswordDto.Password);
                if (isLogin)
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithEmail(emailPasswordDto.Email);
                    if (companyWorker != null)
                    {
                        CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(companyWorker);
                        return Ok(driverInfoDto);
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
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<CompanyWorkerInfoDto> CreateDriver([FromBody]CompanyWorkerRegisterDto driverRegisterDto)
        {
            try
            {
                CompanyWorker newCompanyWorker = _userService.CreateCompanyWorker(driverRegisterDto, Roles.Driver);
                if (newCompanyWorker != null)
                {
                    CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(newCompanyWorker);
                    return Ok(driverInfoDto);
                }
                return BadRequest("Not Added");
            }
            catch (Exception ex)
            { 
                return BadRequest(ex.Message); 
            }


        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<PassengerInfoDto> CreatePassenger([FromBody] PassengerRegisterPanelDto passengerRegisterPanelDto)
        {
            try
            {
                Passenger newPassenger = _userService.CreatePassenger(passengerRegisterPanelDto, Roles.Passenger)
                         ?? throw new ArgumentNullException();
                if (newPassenger != null)
                {
                    PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(newPassenger);
                    return Ok(passengerInfoDto);
                }
                return BadRequest("Not Added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<Passenger> GetPassenger([FromBody] int id)
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
        [HttpPost]
        public IActionResult Test1()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "shuttleasydbserver1.database.windows.net";
                builder.UserID = "emreyilmaz";
                builder.Password = "Easypeasy1";
                builder.InitialCatalog = "ShuttleasyDB";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
      

                    connection.Open();

                    string sql = "SELECT name  FROM company_worker";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return Ok(reader.GetString(0));
                            }
                        }
                    }
                    
                }
                return Ok();
            }
            catch (SqlException e)
            {
                return BadRequest(e.Message);
            }

        }

    }
}
