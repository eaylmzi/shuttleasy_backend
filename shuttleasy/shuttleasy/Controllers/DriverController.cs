

using AutoMapper;
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
using shuttleasy.Models.dto.User.dto;
using shuttleasy.Services;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
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
        private readonly IMapper _mapper;
        public DriverController(IUserService userService , IPassengerLogic passengerLogic,ICompanyWorkerLogic driverLogic,
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
            PasswordEncryption passwordEncryption = new PasswordEncryption();
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
                    return BadRequest("The driver not found in list");
                }
                return BadRequest("Email and password not correct");
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Driver}")]
        public ActionResult<CompanyWorkerInfoDto> UpdateDriver(DriverProfileDto driverProfileDto)
        {
            try
            {               
                CompanyWorker? companyWorkerFromRequestToken = GetCompanyWorkerFromRequestToken();
                if(companyWorkerFromRequestToken != null)
                {

                    CompanyWorker? updatedDriver = _userService.UpdateDriverProfile(companyWorkerFromRequestToken, driverProfileDto);
                    if (updatedDriver != null)
                    {
                        CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(updatedDriver);
                        return Ok(driverInfoDto);
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
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<CompanyWorkerInfoDto> UpdateDriverFromAdmin(DriverProfileDto driverProfileDto,int id)
        {
            try
            {
                CompanyWorker? companyWorkerFromRequestToken = GetCompanyWorkerFromRequestToken();
                if (companyWorkerFromRequestToken != null)
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(id);
                    if(companyWorker != null)
                    {
                        CompanyWorker? updatedDriver = _userService.UpdateDriverProfile(companyWorker, driverProfileDto);
                        if (updatedDriver != null)
                        {
                            CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(updatedDriver);
                            return Ok(driverInfoDto);
                        }
                        return BadRequest("Driver not updated");
                    }
                    return BadRequest("Driver not found");
                }
                return BadRequest("Mistake about token");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
        public ActionResult<CompanyWorkerInfoDto> GetDriver([FromBody] IdDto idDto)
        {
            try
            {
                CompanyWorker? companyWorkerFromRequestToken = GetCompanyWorkerFromRequestToken();
                if (companyWorkerFromRequestToken != null)
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(idDto.Id);
                    if (companyWorker != null)
                    {
                        CompanyWorkerInfoDto companyWorkerInfoDto = _mapper.Map<CompanyWorkerInfoDto>(companyWorker);
                        return Ok(companyWorkerInfoDto);
                    }
                    return BadRequest("Company worker not found");
                }
                return BadRequest("The user that send request not found");
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

        private CompanyWorker? GetCompanyWorkerFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            CompanyWorker? companyWorkerFromToken = _driverLogic.GetCompanyWorkerWithToken(requestToken);
            return companyWorkerFromToken;
        }
    }
}

