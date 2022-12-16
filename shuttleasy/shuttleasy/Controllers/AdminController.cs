﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Login.dto;
using shuttleasy.Models.dto.Passengers.dto;
using shuttleasy.Services;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;

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
                if(_driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken()) != null)
                {
                    CompanyWorker? newCompanyWorker = _userService.CreateCompanyWorker(driverRegisterDto, Roles.Driver);
                    if (newCompanyWorker != null)
                    {
                        CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(newCompanyWorker);
                        return Ok(driverInfoDto);
                    }
                    return BadRequest("Not Added");

                }
                return BadRequest("The driver that send request not found");
               
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
                if (_driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken()) != null)
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
                return BadRequest("The driver that send request not found");
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<Passenger> GetPassenger([FromBody] IdDto idDto)
        {
            try
            {
                int a = GetUserIdFromRequestToken();
                if (_driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken()) != null)
                {
                    return _passengerLogic.GetPassengerWithId(idDto.Id);
                }
                return BadRequest("The user that send request not found");
               
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
                if (_driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken()) != null)
                {
                    return _passengerLogic.GetAllPassengers();
                }
                return BadRequest("The admin that send request not found");
                
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



        private int GetUserIdFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
            string user = jwt.Claims.First(c => c.Type == "id").Value;
            int userId = int.Parse(user);
            return userId;
        }
        private CompanyWorker? GetAdminFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            CompanyWorker? adminFromToken = _driverLogic.GetCompanyWorkerWithToken(requestToken);
            return adminFromToken;
        }

    }
}
