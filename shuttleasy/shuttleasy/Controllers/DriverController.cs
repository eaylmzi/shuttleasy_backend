

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
using shuttleasy.Services.ShuttleServices;
using shuttleasy.LOGIC.Logics.DriversStatistics;
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using shuttleasy.LOGIC.Logics.SessionHistories;
using shuttleasy.Services.NotifService;
using FirebaseAdmin.Messaging;
using shuttleasy.LOGIC.Logics.ShuttleSessions;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DriverController : Controller
    {
        private readonly IUserService _userService;
        private readonly IShuttleService _shuttleService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;       
        private readonly IMapper _mapper;
        private readonly IJoinTableLogic _joinTableLogic;
        private readonly ISessionHistoryLogic _sessionHistoryLogic;
        private readonly IDriversStatisticLogic _driversStatisticLogic;
        private readonly INotificationService _notificationService;
        private readonly IShuttleSessionLogic _shuttleSessionLogic;

        List<ShuttleSession> emptyList = new List<ShuttleSession>();

        public DriverController(IUserService userService , IPassengerLogic passengerLogic,ICompanyWorkerLogic driverLogic,
            IMapper mapper, IJoinTableLogic joinTableLogic, IShuttleService shuttleService,IDriversStatisticLogic driversStatisticLogic,
            ISessionHistoryLogic sessionHistoryLogic, INotificationService notificationService, IShuttleSessionLogic shuttleSessionLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
            _joinTableLogic = joinTableLogic;
            _shuttleService = shuttleService;
            _driversStatisticLogic = driversStatisticLogic;
            _sessionHistoryLogic = sessionHistoryLogic;
            _notificationService = notificationService;
            _shuttleSessionLogic = shuttleSessionLogic;
        }
        [HttpPost]
        public ActionResult<CompanyWorkerInfoDto> Login([FromBody] EmailPasswordDto emailPasswordDto)
        {
            try
            {
                CompanyWorker? companyWorker = _userService.LoginCompanyWorker(emailPasswordDto.Email, emailPasswordDto.Password);
                if (companyWorker != null)
                {
                    CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(companyWorker);
                    return Ok(driverInfoDto);
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
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin}")]
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
        public ActionResult<List<ShuttleDto>> GetSessions()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    int driverId = TokenHelper.GetDriverIdFromRequestToken(Request.Headers);
                    var list = _joinTableLogic.MertimYapmaz(driverId);
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
        public ActionResult<List<PassengerDetailsDto>> GetPassengers([FromBody] IdDto idDto)
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
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public async Task<ActionResult<bool>> ActivateShuttle([FromBody] IdDto sessionDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(sessionDto.Id);
                    if (shuttleSession != null)
                    {
                        shuttleSession.IsActive = true;
                        bool isUpdated = await _shuttleSessionLogic.UpdateAsync(sessionDto.Id, shuttleSession);
                        if (isUpdated)
                        {
                            return Ok(isUpdated);
                        }
                        return BadRequest(isUpdated);
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
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public async Task<ActionResult<bool>> DeactivateShuttle([FromBody] IdDto sessionDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(sessionDto.Id);
                    if (shuttleSession != null)
                    {
                        shuttleSession.IsActive = false;
                        bool isUpdated = await _shuttleSessionLogic.UpdateAsync(sessionDto.Id, shuttleSession);
                        if (isUpdated)
                        {
                            return Ok(isUpdated);
                        }
                        return BadRequest(isUpdated);
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

        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public async Task<ActionResult<bool>> StartShuttle([FromBody] List<SessionPassengerPickupIdDetailsDto> sessionPassengerPickupIdDetailsListDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    NotificationModelToken startingShuttleNotification = new NotificationModelToken();
                    List<string> tokenList = new List<string>();
                    foreach (SessionPassengerPickupIdDetailsDto item in sessionPassengerPickupIdDetailsListDto)
                    {
                        tokenList.Add(item.NotificationToken);
                    }
                    startingShuttleNotification.Token = tokenList;
                    startingShuttleNotification.Title = NotificationTitle.SERVICE_STARTED;
                    startingShuttleNotification.Body = NotificationBody.SERVICE_STARTED;
                    await _notificationService.SendNotificationByToken(startingShuttleNotification);

                    NotificationModelToken notificationModelToken = new NotificationModelToken();
                    List<string> tokenListForFirstPassenger = new List<string>();
                    tokenListForFirstPassenger.Add(sessionPassengerPickupIdDetailsListDto[0].NotificationToken);
                    notificationModelToken.Token = tokenList;
                    notificationModelToken.Title = NotificationTitle.FOR_NEXT_PASSENGER;
                    notificationModelToken.Body = NotificationBody.FOR_NEXT_PASSENGER;
                    var notif = await _notificationService.SendNotificationByToken(notificationModelToken);                  
                    return Ok();


                }
                return Unauthorized(Error.NotMatchedToken);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public async Task<ActionResult<bool>> FinishShuttle([FromBody] IdDto shuttleId)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    if (_sessionHistoryLogic.GetSingleBySessionId(shuttleId.Id) == null)
                    {
                        bool isSessionHistoryAdded = _shuttleService.FinishShuttle(shuttleId.Id);
                        if (isSessionHistoryAdded)
                        {
                            int driverId = TokenHelper.GetDriverIdFromRequestToken(Request.Headers);
                            DriversStatistic? driversStatistic = _driversStatisticLogic.GetSingleDriverId(driverId);

                            if (driversStatistic == null)
                            {
                                DriversStatistic newDriverStatictic = new DriversStatistic();
                                newDriverStatictic.DriverId = driverId;
                                newDriverStatictic.RateCount = 0;
                                newDriverStatictic.RatingAvg = 0;
                                bool isAddedDriverStatictic = _driversStatisticLogic.Add(newDriverStatictic);
                                if (!isAddedDriverStatictic)
                                {
                                    return false;
                                }
                            }
                            List<StartFinishTime> startFinishTime = _joinTableLogic.ShuttleSessionDriverStaticticJoinTables(shuttleId.Id);
                            DriversStatistic statictic = _driversStatisticLogic.GetSingleDriverId(driverId);
                            double differenceInMinutes = (startFinishTime[0].FinalTime - startFinishTime[0].StartTime).TotalHours;
                            statictic.WorkingHours = statictic.WorkingHours + differenceInMinutes;
                            bool isUpdated = await _driversStatisticLogic.UpdateAsync(statictic, driverId);
                            if (isUpdated)
                            {
                                return Ok(isUpdated);
                            }
                            return BadRequest(isUpdated);
                        }
                        return BadRequest(isSessionHistoryAdded);

                    }
                    return BadRequest(Error.AlreadyFinish);
                }
                return Unauthorized(Error.NotMatchedToken);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public async Task<ActionResult<BatchResponse>> TakeNextPerson([FromBody] SessionPassengerPickupIdDetailsDto sessionPassengerPickupIdDetailsDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    NotificationModelToken notificationModelToken = new NotificationModelToken();
                    List<string> tokenList = new List<string>();
                    tokenList.Add(sessionPassengerPickupIdDetailsDto.NotificationToken);
                    notificationModelToken.Token = tokenList;
                    notificationModelToken.Title = NotificationTitle.FOR_NEXT_PASSENGER;
                    notificationModelToken.Body = NotificationBody.FOR_NEXT_PASSENGER;

                    var notif = await _notificationService.SendNotificationByToken(notificationModelToken);
                    return Ok(notif);                 
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

