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

        public ShuttleSessionController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
                    IShuttleBusLogic shuttleBusLogic,IShuttleSessionLogic shuttleSessionLogic, IMapper mapper,
                    IDestinationLogic destinationLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _shuttleBusLogic = shuttleBusLogic;
            _shuttleSessionLogic = shuttleSessionLogic;
            _mapper = mapper;
            _destinationLogic = destinationLogic;
        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin}")]
        public ActionResult<bool> CreateShuttleSession([FromBody] ShuttleSessionDto shuttleSessionDto)
        {
            try
            {
                
                ShuttleSession shuttleSession = _mapper.Map<ShuttleSession>(shuttleSessionDto);
                //string timeStamp = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                //{11.12.2022 16:14:12}
                // byte[] timeStampBytes = Encoding.ASCII.GetBytes(timeStamp); //STRİNG TO TİMESTAMP YAPÇAN
                // shuttleSession.StartTime = DateTime.Now;
                //Shuttlesessionda timestamp ve dateyi sor
                DateTime d = DateTime.Now;
                bool isAdded = _shuttleSessionLogic.CreateShuttleSession(shuttleSession);
                if (isAdded)
                {
                    return Ok(isAdded);
                }
                return BadRequest(isAdded);
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
                bool isAdded = _shuttleSessionLogic.DeleteShuttleSession(idDto.Id);
                if (isAdded)
                {
                    return Ok(isAdded);
                }
                return BadRequest(isAdded);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<ShuttleSession> GetShuttleSession([FromBody] IdDto cpmpanyWorkerId)
        {
            try
            {
                ShuttleSession? shuttleSession = _shuttleSessionLogic.GetShuttleSessionWithCompanyId(cpmpanyWorkerId.Id);
                if (shuttleSession != null)
                {
                    return Ok(shuttleSession);
                }
                return BadRequest("Shuttle session not found");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public ActionResult<List<ShuttleSession>> SearchShuttle([FromBody] SearchDestinationDto searchDestinationDto)
        {
            try
            {
                Destination? destination = _destinationLogic
                        .FindDestinationWithBeginningDestination(searchDestinationDto.BeginningDestination);
                if(destination != null)
                {
                    if (destination.LastDestination.Equals(searchDestinationDto.LastDestination))
                    {
                        List<ShuttleSession>? shuttleSessions = _shuttleSessionLogic.FindSessionWithSpecificLocation(destination.Id);
                        if(shuttleSessions != null)
                        {
                            return Ok(shuttleSessions);
                        }
                        return BadRequest("The bus not found with that destination");
                    }
                    return BadRequest("There is no destination");
                }

                return BadRequest("The destination not in the list");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
    }
}
