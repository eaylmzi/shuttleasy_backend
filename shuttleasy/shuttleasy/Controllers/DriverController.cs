

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.DAL.Models.dto.Login.dto;
using shuttleasy.DAL.Models.dto.Passengers.dto;
using shuttleasy.DAL.Models.dto.User.dto;
using shuttleasy.Resource;
using shuttleasy.Services;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using shuttleasy.LOGIC.Logics.GeoPoints;
using shuttleasy.LOGIC.Logics.JoinTables;

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
        private readonly IJoinTableLogic _joinTableLogic;
        List<ShuttleSession> emptyList = new List<ShuttleSession>();

        public DriverController(IUserService userService , IPassengerLogic passengerLogic,ICompanyWorkerLogic driverLogic,
            IMapper mapper, IJoinTableLogic joinTableLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
            _joinTableLogic = joinTableLogic;
        }
        [HttpPost]
        public ActionResult<CompanyWorkerInfoDto> Login([FromBody] EmailPasswordDto emailPasswordDto)
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
                    return BadRequest(Error.NotFoundDriver);
                }
                return BadRequest(Error.NotCorrectEmailAndPassword);
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
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorkerFromRequestToken = TokenHelper.GetCompanyWorkerFromRequestToken(Request.Headers,_driverLogic);
                    if(companyWorkerFromRequestToken != null)
                    {
                        CompanyWorker? updatedDriver = _userService.UpdateDriverProfile(companyWorkerFromRequestToken, driverProfileDto);
                        if (updatedDriver != null)
                        {
                            CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(updatedDriver);
                            return Ok(driverInfoDto);
                        }
                        return BadRequest(Error.NotUpdatedInformation);
                    }
                    return BadRequest(Error.NotFoundDriver);
                    
                }
                return Unauthorized(Error.NotMatchedToken);

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

                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
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
                        return BadRequest(Error.NotUpdatedInformation);
                    }
                    return BadRequest(Error.NotFoundDriver);
                }
                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<CompanyWorkerInfoDto> GetDriver([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(idDto.Id);
                    if (companyWorker != null)
                    {
                        CompanyWorkerInfoDto companyWorkerInfoDto = _mapper.Map<CompanyWorkerInfoDto>(companyWorker);
                        return Ok(companyWorkerInfoDto);
                    }
                    return BadRequest(Error.NotFoundDriver);
                }
                return BadRequest(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost, Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
        public ActionResult<List<CompanyWorker>> GetAllDriver()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(TokenHelper.GetUserIdFromRequestToken(Request.Headers));
                    if (companyWorker != null)
                    {
                        var list = _driverLogic.GetAllDriverWithCompanyId(companyWorker.CompanyId);
                        return Ok(list);
                    }
                    return BadRequest(Error.NotCreatedUser);

                }
                return Unauthorized(Error.NotMatchedToken);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public ActionResult<List<ShuttleSession>> GetSessions()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    int driverId = TokenHelper.GetDriverIdFromRequestToken(Request.Headers);
                    var list = _joinTableLogic.ShuttleDetailsInnerJoinTables(driverId);
                    if(list != null)
                    {
                        return Ok(list);
                    }
                    return Ok(emptyList);

                  
                }
                return Unauthorized(Error.NotMatchedToken);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
             [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public ActionResult<List<ShuttleSession>> GetPassengers(IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _joinTableLogic.PassengerSessionPassengerJoinTables(idDto.Id);
                    if (list != null)
                    {
                        return Ok(list);
                    }
                    return Ok(emptyList);


                }
                return Unauthorized(Error.NotMatchedToken);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

