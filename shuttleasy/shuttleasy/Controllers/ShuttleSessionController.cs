﻿using AutoMapper;
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
using shuttleasy.Services.ShuttleServices;
using shuttleasy.LOGIC.Logics.PassengerPayments;

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
        private readonly IShuttleService _shuttleService;
        private readonly IPassengerPaymentLogic _passengerPaymentLogic;
        List<ShuttleSession> emptyList = new List<ShuttleSession>();

        public ShuttleSessionController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
                    IShuttleBusLogic shuttleBusLogic,IShuttleSessionLogic shuttleSessionLogic, IMapper mapper, IJoinTableLogic joinTableLogic,
                    ICompanyLogic companyLogic, ISessionPassengerLogic sessionPassengerLogic, IGeoPointLogic geoPointLogic,
                    IPickupAreaLogic pickupAreaLogic, IPickupPointLogic pickupPointLogic, IShuttleService shuttleService,
                    IPassengerPaymentLogic passengerPaymentLogic)
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
            _shuttleService = shuttleService;
            _passengerPaymentLogic = passengerPaymentLogic;

        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin}")]
        public async Task<ActionResult<bool>> CreateShuttleSession([FromBody] ShuttleSessionDto shuttleSessionDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(TokenHelper.GetUserIdFromRequestToken(Request.Headers));
                    if (companyWorker != null)
                    {
                        if (await _shuttleSessionLogic.CheckAllForeignKeysAndUniqueExistAsync(shuttleSessionDto))
                        {
                            ShuttleSession shuttleSession = _mapper.Map<ShuttleSession>(shuttleSessionDto);
                            shuttleSession.ShuttleState = ShuttleState.NOT_STARTED;
                            shuttleSession.RouteState = ShuttleState.NOT_CALCULATED;
                            int? isAdded = _shuttleSessionLogic.AddReturnId(shuttleSession);
                            if (isAdded != 0)
                            {
                                return Ok(isAdded);
                            }
                            return BadRequest(Error.NotFound);
                        }                   
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
        public async Task<ActionResult<ShuttleSession>> ChangeShuttlePrice([FromBody] ShuttleIdPrıce shuttleIdPrice)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(shuttleIdPrice.Id);
                    if(shuttleSession == null)
                    {
                        return BadRequest(Error.NotFoundShuttleSession);
                    }
                    shuttleSession.Price = shuttleIdPrice.Price;
                    bool isShuttleSessionUpdated = await _shuttleSessionLogic.UpdateAsync(shuttleSession.Id, shuttleSession);
                    if (!isShuttleSessionUpdated)
                    {
                        return BadRequest(isShuttleSessionUpdated);
                    }
                    return Ok(shuttleSession);

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
                        bool isDeletedArea = _pickupAreaLogic.DeleteBySessionId(idDto.Id);
                        bool isDeletedSessionPassenger = _sessionPassengerLogic.DeleteBySessionId(idDto.Id);
                        bool isDeletedShuttle = _shuttleSessionLogic.DeleteShuttleSession(idDto.Id);
                        if (isDeletedShuttle)
                        {
                            return Ok(isDeletedShuttle);
                        }
                        return BadRequest(isDeletedShuttle);
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
        public ActionResult<List<ShuttleDetailsGroupDto>> SearchShuttle([FromBody] SearchDestinationDto searchDestinationDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _joinTableLogic.ShuttleDetailsInnerJoinTables(searchDestinationDto.DestinationName);
                    if (list.Count != 0)
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
                    return Ok(shuttleSession);


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
                    PassengerPayment passengerPayment = new PassengerPayment()
                    {
                        PassengerIdentity = userInformation.Id,
                        ShuttleSessionId = sessionPassengerDto.SessionId,
                        IsPaymentVerified = false,
                    };

                    bool isPassengerPaymentAdded = _passengerPaymentLogic.Add(passengerPayment);
                    if (!isPassengerPaymentAdded)
                    {
                        return BadRequest(Error.NotAdded);
                    }
               
                    int userId = TokenHelper.GetUserIdFromRequestToken(Request.Headers);
                    bool isEnrolled = await _shuttleService.EnrollPassenger(sessionPassengerDto, userId);
                    if (isEnrolled)
                    {
                        return Ok(isEnrolled);
                    }
                    return BadRequest(isEnrolled);

                    /*
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
                        else
                        {
                            shuttleSession.PassengerCount = shuttleSession.PassengerCount - 1;
                            await _shuttleSessionLogic.UpdateAsync(shuttleSession.Id, shuttleSession);
                        }
                        return BadRequest(isAdded);


                    }
                    return BadRequest(Error.NotFoundShuttleSession);
                    */
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
        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public async Task<ActionResult<List<int>>> EnrollPassengerMultipleSession([FromBody] SessionPassengerMultipleDto sessionPassengerMultipleDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    foreach (int sessionId in sessionPassengerMultipleDto.SessionIdList)
                    {
                        PassengerPayment passengerPayment = new PassengerPayment()
                        {
                            PassengerIdentity = userInformation.Id,
                            ShuttleSessionId = sessionId,
                            IsPaymentVerified = false,
                        };

                        bool isPassengerPaymentAdded = _passengerPaymentLogic.Add(passengerPayment);
                        if (!isPassengerPaymentAdded)
                        {
                            return BadRequest(Error.NotAdded);
                        }
                    }
                   


                    List<int> shuttleList = new List<int>();

                    for (int i = 0;i < sessionPassengerMultipleDto.SessionIdList.Count; i++)
                    {
                        GeoPoint geoPoint = new GeoPoint();
                        geoPoint.Longtitude = sessionPassengerMultipleDto.Longitude;
                        geoPoint.Latitude = sessionPassengerMultipleDto.Latitude;
                        int? geoPointId = _geoPointLogic.FindByCoordinate(geoPoint.Longtitude, geoPoint.Latitude);
                        int userId = TokenHelper.GetUserIdFromRequestToken(Request.Headers);
                        ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(sessionPassengerMultipleDto.SessionIdList[i]);
                        if (shuttleSession != null)
                        {
                                       
                            shuttleSession.PassengerCount = shuttleSession.PassengerCount + 1;
                            await _shuttleSessionLogic.UpdateAsync(shuttleSession.Id, shuttleSession);
                            SessionPassenger sessionPassenger = _mapper.Map<SessionPassenger>(sessionPassengerMultipleDto);
                            sessionPassenger.SessionId = sessionPassengerMultipleDto.SessionIdList[i];

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
                            bool isAdded =_sessionPassengerLogic.Add(sessionPassenger);
                            if (isAdded)
                            {
                                shuttleList.Add(sessionPassengerMultipleDto.SessionIdList[i]);
                            }
                            else
                            {
                                shuttleSession.PassengerCount = shuttleSession.PassengerCount - 1 ;
                                await _shuttleSessionLogic.UpdateAsync(shuttleSession.Id, shuttleSession);
                            }
                            



                        }
                        

                    }
                    return Ok(shuttleList);

                }
                return Unauthorized(Error.NotMatchedToken);
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
        public ActionResult<List<ShuttleDetailsGroupDto>> GetShuttleByGeoPoint([FromBody] SessionGeoPointsDto geoPointDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _joinTableLogic.ShuttleDetailsByGeoPointInnerJoinTables(geoPointDto.Longtitude, geoPointDto.Latitude);
                    if (list.Count != 0)
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
