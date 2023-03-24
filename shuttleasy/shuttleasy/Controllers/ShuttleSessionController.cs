using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.ShuttleBuses;
using shuttleasy.LOGIC.Logics;
using shuttleasy.DAL.Models.dto.ShuttleBuses.dto;
using shuttleasy.Services;
using Newtonsoft.Json.Linq;
using shuttleasy.LOGIC.Logics.ShuttleSessions;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.ShuttleSessions.dto;
using System.Text;
using shuttleasy.DAL.Models.dto.Driver.dto;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using shuttleasy.DAL.Models.dto.Passengers.dto;
using System.Collections.Generic;
using shuttleasy.LOGIC.Logics.Companies;
using shuttleasy.Resource;
using shuttleasy.DAL.EFRepositories.ShuttleSessionSearch;
using shuttleasy.LOGIC.Logics.ShuttleSessionSearch;
using shuttleasy.LOGIC.Logics.JoinTables;
using shuttleasy.DAL.Models.dto.SessionPassengers.dto;
using shuttleasy.LOGIC.Logics.SessionPassengers;
using shuttleasy.LOGIC.Logics.GeoPoints;
using shuttleasy.DAL.Models.dto.GeoPoints.dto;
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using shuttleasy.LOGIC.Logics.PickupPoints;
using Microsoft.EntityFrameworkCore;
using shuttleasy.LOGIC.Logics.PickupAreas;

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
        private readonly IJoinTableLogic _joinTableLogic;
        private readonly ICompanyLogic _companyLogic;
        private readonly ISessionPassengerLogic _sessionPassengerLogic;
        private readonly IGeoPointLogic _geoPointLogic;
        private readonly IPickupAreaLogic _pickupAreaLogic;
        private readonly IPickupPointLogic _pickupPointLogic;

        public ShuttleSessionController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
                    IShuttleBusLogic shuttleBusLogic,IShuttleSessionLogic shuttleSessionLogic, IMapper mapper, IJoinTableLogic joinTableLogic,
                    ICompanyLogic companyLogic, ISessionPassengerLogic sessionPassengerLogic, IGeoPointLogic geoPointLogic,
                    IPickupAreaLogic pickupAreaLogic, IPickupPointLogic pickupPointLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _shuttleBusLogic = shuttleBusLogic;
            _shuttleSessionLogic = shuttleSessionLogic;
            _mapper = mapper;
            _joinTableLogic = joinTableLogic;
            _companyLogic = companyLogic;
            _sessionPassengerLogic = sessionPassengerLogic;
            _geoPointLogic = geoPointLogic;
            _pickupAreaLogic = pickupAreaLogic;
            _pickupPointLogic = pickupPointLogic;

        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin}")]
        public ActionResult<bool> CreateShuttleSession([FromBody] ShuttleSessionDto shuttleSessionDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(TokenHelper.GetUserIdFromRequestToken(Request.Headers));
                    if (companyWorker != null)
                    {
                        ShuttleSession shuttleSession = _mapper.Map<ShuttleSession>(shuttleSessionDto);
                        //string timeStamp = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                        //{11.12.2022 16:14:12}
                        // byte[] timeStampBytes = Encoding.ASCII.GetBytes(timeStamp); //STRİNG TO TİMESTAMP YAPÇAN
                        // shuttleSession.StartTime = DateTime.Now;
                        //Shuttlesessionda timestamp ve dateyi sor
                        int? isAdded = _shuttleSessionLogic.AddReturnId(shuttleSession);
                        if (isAdded != 0)
                        {
                            return Ok(isAdded);
                        }
                        return BadRequest(Error.NotFound);
                    }
                    return BadRequest(Error.NotFoundUser);

                }
                return Unauthorized(Error.NotMatchedToken);

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
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(TokenHelper.GetUserIdFromRequestToken(Request.Headers));
                    if (companyWorker != null)
                    {
                        bool isDeleted = _pickupAreaLogic.DeleteBySessionId(idDto.Id);
                        if (isDeleted)
                        {
                            bool isDeletedShuttle = _shuttleSessionLogic.DeleteShuttleSession(idDto.Id);
                            if (isDeletedShuttle)
                            {
                                return Ok(isDeletedShuttle);
                            }
                            return BadRequest(isDeletedShuttle);

                        }
                        return BadRequest(Error.NotDeletedPickupArea);
                       
                    }
                    return BadRequest(Error.NotFoundUser);
                }
                return Unauthorized(Error.NotMatchedToken);



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
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(TokenHelper.GetUserIdFromRequestToken(Request.Headers));
                    if (companyWorker != null)
                    {
                        var list = _shuttleSessionLogic.GetAllSessionsWithCompanyId(companyWorker.CompanyId);
                        return Ok(list);
                    }
                    return BadRequest(Error.NotFoundUser);
                }
                return Unauthorized(Error.NotMatchedToken);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<List<ShuttleDetailsDto>> SearchShuttle([FromBody] SearchDestinationDto searchDestinationDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _joinTableLogic.ShuttleDetailsInnerJoinTables(searchDestinationDto.DestinationName);
                    if (list.Capacity != 0)
                    {
                        return Ok(list);
                    }

                    return BadRequest(Error.NotFoundShuttleSession);
                }
                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<List<ShuttleSession>> GetShuttle([FromBody] IdDto idDto )
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(idDto.Id);
                    if (shuttleSession != null)
                    {
                        return Ok(shuttleSession);
                    }
                    return BadRequest(Error.NotFoundShuttleSession);


                }
                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /*
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<List<ShuttleSession>> GetLocationList([FromBody] IdDto sessionDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    int sessionId = sessionDto.Id;
                    var list = _joinTableLogic.SessionGeoPointsInnerJoinTables(sessionId);
                    if (list != null) 
                    {
                        return Ok(list);
                    }
                    return BadRequest(Error.EmptyList);

                }
                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        */

        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public async Task<ActionResult<bool>> EnrollPassenger([FromBody] SessionPassengerDto sessionPassengerDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(sessionPassengerDto.SessionId);
                    if(shuttleSession != null)
                    {
                        GeoPoint geoPoint = new GeoPoint();
                        geoPoint.Longtitude = sessionPassengerDto.Longitude;
                        geoPoint.Latitude = sessionPassengerDto.Latitude;
                        int? geoPointId = _geoPointLogic.FindByCoordinate(geoPoint.Longtitude, geoPoint.Latitude);

                        int userId = TokenHelper.GetUserIdFromRequestToken(Request.Headers);
                        shuttleSession.PassengerCount = shuttleSession.PassengerCount + 1;
                        await _shuttleSessionLogic.UpdateAsync(shuttleSession.Id, shuttleSession);
                        SessionPassenger sessionPassenger = _mapper.Map<SessionPassenger>(sessionPassengerDto);
                        PickupPoint pickupPoint = new PickupPoint();
                        if (geoPointId != null)
                        {
                            pickupPoint.GeoPointId = (int)geoPointId;
                            pickupPoint.UserId = userId;
                            int? pickUpId = _pickupPointLogic.AddReturnId(pickupPoint);
                            sessionPassenger.PickupId = pickUpId;
                        }
                        else
                        {
                            int? addedGeoPointId = _geoPointLogic.AddReturnId(geoPoint);
                            pickupPoint.GeoPointId = (int)addedGeoPointId;
                            pickupPoint.UserId = userId;
                            int? pickUpId = _pickupPointLogic.AddReturnId(pickupPoint);
                            sessionPassenger.PickupId = pickUpId;
                        }

                        //BURAYI SONRA SİLECEKSİN
                        sessionPassenger.PickupOrderNum = null;
                        sessionPassenger.PickupState = null;
                        sessionPassenger.EstimatedPickupTime = null;
                        bool isAdded = _sessionPassengerLogic.Add(sessionPassenger);
                        if (isAdded)
                        {
                            return Ok(isAdded);

                        }
                        return BadRequest(isAdded);


                    }
                    return BadRequest(Error.NotFoundShuttleSession);

                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException(Error.AlreadyFound);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
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
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<List<ShuttleSession>> GetShuttleByGeoPoint([FromBody] SessionGeoPointsDto geoPointDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _joinTableLogic.ShuttleDetailsByGeoPointInnerJoinTables(geoPointDto.Longtitude, geoPointDto.Latitude);
                    if (list.Capacity != 0)
                    {
                        return Ok(list);
                    }

                    return BadRequest(Error.NotFoundShuttleSession);
                }
                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /*

         [HttpPost]
         public ActionResult<List<ShuttleSessionSearchDto>> LAAAA(string lastPoint)
         {
             ShuttleSessionSearchLogic shuttleSessionSearchLogic = new ShuttleSessionSearchLogic();
             var list = shuttleSessionSearchLogic.InnerJoinTables(lastPoint);

             return Ok(list);
         }

         */





        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
    }
}
