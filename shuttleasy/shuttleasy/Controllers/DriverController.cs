

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
using shuttleasy.DAL.Models.dto.Session.dto;
using ShuttleRoute;
using shuttleasy.LOGIC.Logics.SessionPassengers;
using System.Text;
using shuttleasy.DAL.Models.dto.ShuttleSessions.dto;
using shuttleasy.LOGIC.Logics.PassengerPayments;

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
        private readonly ISessionPassengerLogic _sessionPassengerLogic;
        private readonly IGeoPointLogic _geoPointLogic;
        private readonly IPassengerPaymentLogic _passengerPaymentLogic;

        List<ShuttleSession> emptyList = new List<ShuttleSession>();

        public DriverController(IUserService userService , IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
            IMapper mapper, IJoinTableLogic joinTableLogic, IShuttleService shuttleService, IDriversStatisticLogic driversStatisticLogic,
            ISessionHistoryLogic sessionHistoryLogic, INotificationService notificationService, IShuttleSessionLogic shuttleSessionLogic,
            ISessionPassengerLogic sessionPassengerLogic, IGeoPointLogic geoPointLogic, IPassengerPaymentLogic passengerPaymentLogic)
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
            _sessionPassengerLogic = sessionPassengerLogic;
            _geoPointLogic = geoPointLogic;
            _passengerPaymentLogic = passengerPaymentLogic;
        }
        [HttpPost]
        public async Task<ActionResult<CompanyWorkerInfoDto>> Login([FromBody] EmailPasswordNotifDto emailPasswordNotifDto)
        {
            try
            {
                CompanyWorker? companyWorker = _userService.LoginCompanyWorker(emailPasswordNotifDto.Email, emailPasswordNotifDto.Password);
                if (companyWorker == null)
                {
                    return BadRequest(Error.NotCorrectEmailAndPassword);               
                }
                if(emailPasswordNotifDto.NotificationToken != null)
                {
                    companyWorker.NotificationToken = emailPasswordNotifDto.NotificationToken;
                    bool isCompanyWorkerUpdated = await _driverLogic.UpdateAsync(companyWorker.Id,companyWorker);
                    if (!isCompanyWorkerUpdated)
                    {
                        return BadRequest(Error.NotUpdatedInformation);
                    }
                }

                CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(companyWorker);
                return Ok(driverInfoDto);

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
        public async Task<ActionResult<ShuttleManager>> CalculateRoute([FromBody] IdDto shuttleId)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    ShuttleManager shuttleManager = GetShuttleManager(shuttleId.Id);
                    if(shuttleManager == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    ShuttleManager? calculatedShuttleManager = await calculateRoute(shuttleManager);
                    if(calculatedShuttleManager == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    return Ok(calculatedShuttleManager);
                }
                return Unauthorized(Error.NotMatchedToken);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public async Task<ActionResult<BatchResponse>> StartShuttle([FromBody] IdDto shuttleId)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    ShuttleManager shuttleManager = new ShuttleManager();
                    ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(shuttleId.Id);
                    if (shuttleSession == null)
                    {
                        return BadRequest(Error.NotFoundShuttleSession);
                    }
                    if (shuttleSession.RouteState == ShuttleState.NOT_CALCULATED)
                    {
                        ShuttleManager? newShuttleManager = _userService.GetPassengersLocation(shuttleId.Id);
                        if (newShuttleManager == null)
                        {
                            return BadRequest(Error.NotFound);
                        }
                        ShuttleManager? calculatedShuttleManager = await calculateRoute(newShuttleManager);
                        if (calculatedShuttleManager == null)
                        {
                            return BadRequest(Error.NotFound);
                        }
                        shuttleManager = calculatedShuttleManager;
                    }
                    else
                    {
                        shuttleManager = GetShuttleManagerByPickupOrder(shuttleId.Id);
                    }
                    
                    shuttleSession.ShuttleState = ShuttleState.ON_ROAD;
                    bool isUpdated = await _shuttleSessionLogic.UpdateAsync(shuttleManager.ShuttleRouteDto.Id, shuttleSession);
                    if (!isUpdated)
                    {
                        return BadRequest(isUpdated);
                    }

                    NotificationModelToken startingShuttleNotification = new NotificationModelToken();
                    List<string> tokenList = new List<string>();
                    foreach (PassengerRouteDto item in shuttleManager.PassengerRouteDto)
                    {
                        tokenList.Add(item.NotificationToken);
                    }
                    startingShuttleNotification.Token = tokenList;
                    startingShuttleNotification.Title = NotificationTitle.SERVICE_STARTED;
                    startingShuttleNotification.Body = NotificationBody.SERVICE_STARTED;
                    BatchResponse batchResponse = await _notificationService.SendNotificationByToken(startingShuttleNotification);               
                    return Ok(batchResponse);
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
                    ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(shuttleId.Id);
                    if (shuttleSession == null)
                    {
                        return BadRequest(Error.NotFoundShuttleSession);
                    }
                    shuttleSession.ShuttleState = ShuttleState.FINISHED;
                    bool isUpdatedShuttleSession = await _shuttleSessionLogic.UpdateAsync(shuttleId.Id, shuttleSession);
                    if (!isUpdatedShuttleSession)
                    {
                        return BadRequest(isUpdatedShuttleSession);
                    }
                                       
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
        public async Task<ActionResult<List<DateTime?>>> TakeNextPerson([FromBody] IdDto shuttleId)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    ShuttleManager? shuttleManager = GetShuttleManagerByPickupOrder(shuttleId.Id);
                    if (shuttleManager == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(shuttleId.Id);
                    if (shuttleSession == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    int? lastPerson = shuttleSession.LastPickupIndex;
                    if(lastPerson == null)
                    {
                        lastPerson = 1;
                    }
                    else
                    {
                        lastPerson = lastPerson + 1;
                    }
                    shuttleSession.LastPickupIndex = lastPerson;
                    bool isShuttleSessionUpdated = await _shuttleSessionLogic.UpdateAsync(shuttleId.Id, shuttleSession);
                    if (!isShuttleSessionUpdated)
                    {
                        return BadRequest(Error.NotUpdatedInformation);
                    }
                    TimeSpan? extraTime = DateTime.Now - shuttleManager.PassengerRouteDto[(int)lastPerson].EstimatedArriveTime;
                    List<DateTime?> dateTimeList = new List<DateTime?>();
                    for (int i = (int)lastPerson; i < shuttleManager.PassengerRouteDto.Count; i++ )
                    {
                        SessionPassenger sessionPassenger = _joinTableLogic.GetSessionPassengerJoinTables(shuttleManager.PassengerRouteDto[i].UserId, shuttleManager.ShuttleRouteDto.Id)[0];
                        sessionPassenger.EstimatedPickupTime = shuttleManager.PassengerRouteDto[i].EstimatedArriveTime + extraTime;
                        bool isSessionPassengerUpdated = await _sessionPassengerLogic.UpdateAsync(sessionPassenger.Id, sessionPassenger);
                        if (!isSessionPassengerUpdated)
                        {
                            return BadRequest(Error.NotUpdatedInformation);
                        }
                        dateTimeList.Add(sessionPassenger.EstimatedPickupTime);
                    }



                    NotificationModelToken notificationModelToken = new NotificationModelToken();
                    List<string> tokenList = new List<string>();
                    tokenList.Add(shuttleManager.PassengerRouteDto[(int)lastPerson].NotificationToken);
                    notificationModelToken.Token = tokenList;
                    notificationModelToken.Title = NotificationTitle.FOR_NEXT_PASSENGER;
                    notificationModelToken.Body = NotificationBody.FOR_NEXT_PASSENGER;

                    var notif = await _notificationService.SendNotificationByToken(notificationModelToken);
                    return Ok(dateTimeList);                 
                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver}")]
        public ActionResult<PassengerInfoPriceDto> GetPaymentInfo([FromBody] PassengerQrStringDto passengerQrStringDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(userInformation.Id);
                    if (companyWorker == null)
                    {
                        return BadRequest(Error.NotFoundDriver);
                    }
                    Passenger? passenger = _passengerLogic.GetPassengerQr(passengerQrStringDto.QrString);
                    if(passenger == null)
                    {
                        return BadRequest(Error.NotFoundPassenger);
                    }
                    List<ShuttleIdPrıce> shuttleIdPrıceList = _joinTableLogic.PassengerPaymentJoinTables(passenger.Id,companyWorker.CompanyId);
                    if (shuttleIdPrıceList == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    double priceOfShuttles = 0;
                    foreach (ShuttleIdPrıce shuttleIdPrıce in shuttleIdPrıceList)
                    {
                        priceOfShuttles = priceOfShuttles + shuttleIdPrıce.Price;
                    }
                    PassengerInfoPriceDto passengerInfoPriceDto = new PassengerInfoPriceDto()
                    {
                        Id = passenger.Id,
                        Price = priceOfShuttles,
                        Name = passenger.Name,
                        Surname = passenger.Surname,
                        ProfilePic = passenger.ProfilePic,
                    };
                    return Ok(passengerInfoPriceDto);
                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver}")]
        public async Task<ActionResult<bool>> ConfirmPayment([FromBody] PassengerInfoPriceDto passengerInfoPriceDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(userInformation.Id);
                    if (companyWorker == null)
                    {
                        return BadRequest(Error.NotFoundDriver);
                    }
                    List<PassengerPayment> passengerPaymentList = _joinTableLogic.GetSessionPassengerListJoinTables(passengerInfoPriceDto.Id,companyWorker.CompanyId);
                    if (passengerPaymentList == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    foreach (PassengerPayment passengerPayment in passengerPaymentList)
                    {
                        passengerPayment.IsPaymentVerified = true;
                        passengerPayment.PaymentDate = DateTime.Now;
                        bool isPassengerPaymentUpdated = await _passengerPaymentLogic.UpdateAsync(passengerPayment.ShuttleSessionId, passengerPayment);
                        if (!isPassengerPaymentUpdated)
                        {
                            return BadRequest(isPassengerPaymentUpdated);
                        }
                    }
                    Passenger? passenger = _passengerLogic.GetPassengerWithId(passengerInfoPriceDto.Id);
                    if (passenger == null)
                    {
                        return BadRequest(Error.NotFoundPassenger);
                    }
                    NotificationModelToken notificationModelToken = new NotificationModelToken();
                    List<string> tokenList = new List<string>();
                    tokenList.Add(passenger.NotificationToken);
                    notificationModelToken.Token = tokenList;
                    notificationModelToken.Title = NotificationTitle.PAYMENT_VERIFIED;
                    notificationModelToken.Body = $"Your payment of {passengerInfoPriceDto.Price} TL has been successfully processed. We wish you a pleasant journey.";

                    var notif = await _notificationService.SendNotificationByToken(notificationModelToken);
                    return Ok(true);
                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private ShuttleManager GetShuttleManagerByPickupOrder(int shuttleId)
        {
            List<PassengerRouteDto> passengerRouteDto = _joinTableLogic.PassengerRouteByPickupOrderJoinTables(shuttleId);
            ShuttleSession? shuttle = _shuttleSessionLogic.FindShuttleSessionById(shuttleId);
            if (shuttle == null)
            {
                return null;
            }
            ShuttleRouteDto shuttleRouteDto = new ShuttleRouteDto()
            {
                Id = shuttle.Id,
                StartTime = shuttle.StartTime,
                StartGeopoint = _geoPointLogic.Find((int)shuttle.StartGeopoint),
                FinalGeopoint = _geoPointLogic.Find((int)shuttle.FinalGeopoint),


            };
            ShuttleManager shuttleManager = new ShuttleManager()
            {
                PassengerRouteDto = passengerRouteDto,
                ShuttleRouteDto = shuttleRouteDto,
            };
            return shuttleManager;
        }
        private ShuttleManager GetShuttleManager(int shuttleId)
        {
            List<PassengerRouteDto> passengerRouteDto = _joinTableLogic.PassengerRouteJoinTables(shuttleId);
            ShuttleSession? shuttle = _shuttleSessionLogic.FindShuttleSessionById(shuttleId);
            if (shuttle == null)
            {
                return null;
            }
            ShuttleRouteDto shuttleRouteDto = new ShuttleRouteDto()
            {
                Id = shuttle.Id,
                StartTime = shuttle.StartTime,
                StartGeopoint = _geoPointLogic.Find((int)shuttle.StartGeopoint),
                FinalGeopoint = _geoPointLogic.Find((int)shuttle.FinalGeopoint),


            };
            ShuttleManager shuttleManager = new ShuttleManager()
            {
                PassengerRouteDto = passengerRouteDto,
                ShuttleRouteDto = shuttleRouteDto,
            };
            return shuttleManager;
        }

        [HttpPost, Authorize(Roles = $"{Roles.Driver}")]
        public async Task<ActionResult<CompanyWorker>> UploadImage(IFormFile file)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = TokenHelper.GetCompanyWorkerFromRequestToken(Request.Headers, _driverLogic);
                    if (companyWorker == null)
                    {
                        return BadRequest(Error.NotFoundPassenger);
                    }

                    if (file != null && file.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var extension = Path.GetExtension(file.FileName);
                        if (!allowedExtensions.Contains(extension.ToLower()))
                        {
                            return BadRequest("Invalid file type. Only JPG, JPEG and PNG files are allowed.");
                        }

                        try
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                var base64String = Convert.ToBase64String(fileBytes);
                                byte[] byteArray = Encoding.UTF8.GetBytes(base64String);
                                companyWorker.ProfilePic = byteArray;
                                bool isPassengerUpdated = await _driverLogic.UpdateAsync(companyWorker.Id, companyWorker);
                                if (isPassengerUpdated)
                                {
                                    return Ok(companyWorker);
                                }
                                return BadRequest(isPassengerUpdated);

                            }
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                        }
                    }

                    return BadRequest("Please select a file to upload.");

                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver}")]
        public IActionResult DisplayImage([FromBody] byte[] photo)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    if (photo != null)
                    {
                        string str = Encoding.UTF8.GetString(photo);
                        var imageData = Convert.FromBase64String(str);
                        return Ok(imageData);
                        //return File(imageData, "image/jpg"); // veya "image/png" veya "image/gif" gibi uygun MIME türünü belirtebilirsiniz

                    }
                    else
                    {
                        return BadRequest("There is no pic.");
                    }


                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        private async Task<ShuttleManager> calculateRoute(ShuttleManager shuttleManager)
        {
            ShuttleManager calculatedShuttleManager = await ShuttleRouteManager.CalculateRouteAsync(shuttleManager);
            SessionPassenger? sessionPassenger = new SessionPassenger();
            for (int i = 0; calculatedShuttleManager.PassengerRouteDto.Count > i; i++)
            {
               
                sessionPassenger = _joinTableLogic.GetSessionPassengerJoinTables(calculatedShuttleManager.PassengerRouteDto[i].UserId, calculatedShuttleManager.ShuttleRouteDto.Id)[0];
                if (sessionPassenger == null)
                {
                    return null;
                }
                sessionPassenger.PickupOrderNum = i + 1;
                sessionPassenger.EstimatedPickupTime = calculatedShuttleManager.PassengerRouteDto[i].EstimatedArriveTime;
                bool isSessionPassengerUpdated = await _sessionPassengerLogic.UpdateAsync(sessionPassenger.Id, sessionPassenger);
                if (!isSessionPassengerUpdated)
                {
                    return null;
                }
                sessionPassenger = null;
            }
            ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(shuttleManager.ShuttleRouteDto.Id);
            if (shuttleSession == null)
            {
                return null;
            }
            shuttleSession.RouteState = ShuttleState.CALCULATED;
            bool isShuttleUpdated = await _shuttleSessionLogic.UpdateAsync(shuttleManager.ShuttleRouteDto.Id, shuttleSession);
            if (!isShuttleUpdated)
            {
                return null;
            }
            return calculatedShuttleManager;

        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<bool> IsNotificationTokenEqual([FromBody] PassengerNotificationTokenDto passengerNotificationTokenDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetSingle(passengerNotificationTokenDto.Id);
                    if (companyWorker != null)
                    {
                        if (passengerNotificationTokenDto.NotificationToken == companyWorker.NotificationToken)
                        {
                            return true;
                        }
                        return false;
                    }
                    return BadRequest(Error.NotFoundPassenger);
                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver}")]
        public async Task<ActionResult<bool>> UpdateNotificationToken([FromBody] PassengerNotificationTokenDto passengerNotificationTokenDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetSingle(passengerNotificationTokenDto.Id);
                    if (companyWorker != null)
                    {
                        companyWorker.NotificationToken = passengerNotificationTokenDto.NotificationToken;
                        bool isUpdated = await _driverLogic.UpdateAsync(passengerNotificationTokenDto.Id, companyWorker);
                        if (isUpdated)
                        {
                            return true;
                        }
                        return false;
                    }
                    return BadRequest(Error.NotFoundPassenger);
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

