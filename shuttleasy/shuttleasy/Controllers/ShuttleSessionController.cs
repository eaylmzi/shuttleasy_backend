using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.ShuttleBuses;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.ShuttleBuses.dto;
using shuttleasy.Services;
using Newtonsoft.Json.Linq;
using shuttleasy.LOGIC.Logics.ShuttleSessions;
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.LOGIC.Logics.Destinations;
using shuttleasy.Models.dto.ShuttleSessions.dto;
using System.Text;
using shuttleasy.Models.dto.Driver.dto;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using shuttleasy.Models.dto.Passengers.dto;
using System.Collections.Generic;
using shuttleasy.LOGIC.Logics.Companies;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShuttleSessionController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IShuttleBusLogic _shuttleBusLogic;
        private readonly IShuttleSessionLogic _shuttleSessionLogic;
        private readonly IMapper _mapper;
        private readonly IDestinationLogic _destinationLogic;
        private readonly ICompanyLogic _companyLogic;

        public ShuttleSessionController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
                    IShuttleBusLogic shuttleBusLogic,IShuttleSessionLogic shuttleSessionLogic, IMapper mapper,
                    IDestinationLogic destinationLogic,ICompanyLogic companyLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _shuttleBusLogic = shuttleBusLogic;
            _shuttleSessionLogic = shuttleSessionLogic;
            _mapper = mapper;
            _destinationLogic = destinationLogic;
            _companyLogic = companyLogic;
        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin}")]
        public ActionResult<bool> CreateShuttleSession([FromBody] ShuttleSessionDto shuttleSessionDto)
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken());
                    if (companyWorker != null)
                    {
                        ShuttleSession shuttleSession = _mapper.Map<ShuttleSession>(shuttleSessionDto);
                        //string timeStamp = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                        //{11.12.2022 16:14:12}
                        // byte[] timeStampBytes = Encoding.ASCII.GetBytes(timeStamp); //STRİNG TO TİMESTAMP YAPÇAN
                        // shuttleSession.StartTime = DateTime.Now;
                        //Shuttlesessionda timestamp ve dateyi sor
                        bool isAdded = _shuttleSessionLogic.CreateShuttleSession(shuttleSession);
                        if (isAdded)
                        {
                            return Ok(isAdded);
                        }
                        return BadRequest(isAdded);
                    }
                    return BadRequest("The user that send request not found");

                }
                return Unauthorized("Mistake about token");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin}")]
        public ActionResult<bool> DeleteShuttleSession([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken());
                    if (companyWorker != null)
                    {
                        bool isAdded = _shuttleSessionLogic.DeleteShuttleSession(idDto.Id);
                        if (isAdded)
                        {
                            return Ok(isAdded);
                        }
                        return BadRequest(isAdded);
                    }
                    return BadRequest("The user that send request not found");
                }
                return Unauthorized("Mistake about token");



            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<List<ShuttleSession>> GetAllSessions()
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken());
                    if (companyWorker != null)
                    {
                        var list = _shuttleSessionLogic.GetAllSessionsWithCompanyId(companyWorker.CompanyId);
                        return Ok(list);
                    }
                    return BadRequest("The user that send request not found");

                }
                return Unauthorized("Mistake about token");


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<List<ShuttleSessionSearchDto>> SearchShuttle([FromBody] SearchDestinationDto searchDestinationDto)
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken());
                    if (companyWorker != null)
                    {
                        Destination? destination = _destinationLogic
                          .FindDestinationWithBeginningDestination(searchDestinationDto.BeginningDestination);
                        if (destination != null)
                        {
                            if (destination.LastDestination.Equals(searchDestinationDto.LastDestination))
                            {
                                List<ShuttleSession>? shuttleSessions = _shuttleSessionLogic.FindSessionsWithSpecificLocation(destination.Id);
                                ShuttleSessionSearchDto shuttleSessionSearchDto = new ShuttleSessionSearchDto();
                                List<ShuttleSessionSearchDto>? shuttleSessionDtoList = new List<ShuttleSessionSearchDto>();
                                if (shuttleSessions != null)
                                {
                                    foreach(var item in shuttleSessions)
                                    {
                                        shuttleSessionSearchDto = _mapper.Map<ShuttleSessionSearchDto>(item);
                                        shuttleSessionSearchDto.CompanyName = _companyLogic.GetCompanyNameWithCompanyId(item.CompanyId);
                                        shuttleSessionSearchDto.BusLicensePlate = _shuttleBusLogic.GetBusLicensePlateWithBusId(item.BusId);
                                        shuttleSessionDtoList.Add(shuttleSessionSearchDto);
                                    }
                                    return Ok(shuttleSessionDtoList);
                                }
                                return BadRequest("The bus not found with that destination");
                            }
                            return BadRequest("There is no destination");
                        }

                        return BadRequest("The destination not in the list");

                    }
                    return BadRequest("The user that send request not found");

                }
                return Unauthorized("Mistake about token");



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


        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
    }
}
