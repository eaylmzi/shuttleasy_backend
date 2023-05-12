using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.SessionHistories;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.SessionPassengers;
using Microsoft.AspNetCore.Authorization;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.GeoPoints.dto;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Resource;
using shuttleasy.DAL.Models.dto.SessionPassengers.dto;
using shuttleasy.LOGIC.Logics.JoinTables;
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using shuttleasy.DAL.Models.dto.Session.dto;
using shuttleasy.LOGIC.Logics.ShuttleSessions;
using shuttleasy.LOGIC.Logics.GeoPoints;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SessionPassengerController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly ISessionPassengerLogic _sessionPassengerLogic;
        private readonly IJoinTableLogic _joinTableLogic;
        private readonly IShuttleSessionLogic _shuttleSessionLogic;
        private readonly IGeoPointLogic _geoPointLogic;
        private readonly IMapper _mapper;
        public SessionPassengerController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
            ISessionPassengerLogic sessionPassengerLogic, IJoinTableLogic joinTableLogic, IShuttleSessionLogic shuttleSessionLogic,
           IMapper mapper, IGeoPointLogic geoPointLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _sessionPassengerLogic = sessionPassengerLogic;
            _mapper = mapper;
            _joinTableLogic = joinTableLogic;
            _shuttleSessionLogic = shuttleSessionLogic;
            _geoPointLogic = geoPointLogic;
        }
        
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<bool> AddSessionPassenger([FromBody] SessionPassengerDto sessionPassengerDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    SessionPassenger sessionPassenger = _mapper.Map<SessionPassenger>(sessionPassengerDto);
                    bool isAdded = _sessionPassengerLogic.Add(sessionPassenger);
                    if (isAdded)
                    {
                        return Ok(isAdded);
  
                    }
                    return BadRequest(isAdded);

                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<bool> DeleteSessionPassenger([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    bool isAdded = _sessionPassengerLogic.Delete(idDto.Id);
                    if (isAdded)
                    {
                        return Ok(isAdded);
                    }
                    return BadRequest(isAdded);


                }
                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<SessionPassengerAndPassengerId> GetSessionListByShuttleId([FromBody] IdDto shuttleId)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _joinTableLogic.GetSessionPassengerAndPassengerIdJoinTables(shuttleId.Id);
                    if (list.Count == 0)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    return Ok(list);

                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<ShuttleManager> GetPassengersLocation([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    ShuttleManager? shuttleManager = _userService.GetPassengersLocation(idDto.Id);
                    if (shuttleManager == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    return Ok(shuttleManager);

                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult<List<SessionPassenger>> AAA([FromBody] IdDto idDto)
        {
            try
            {
                return _sessionPassengerLogic.GetListById(248);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
