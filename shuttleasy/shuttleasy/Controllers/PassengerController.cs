using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.JwtToken;
using shuttleasy.LOGIC.Logics;
using shuttleasy.DAL.Models.dto.Passengers.dto;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using shuttleasy.Mail;
using shuttleasy.Services;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.Login.dto;
using shuttleasy.DAL.Models.dto.User.dto;
using System.Data;
using Microsoft.Net.Http.Headers;
using shuttleasy.Resource;
using shuttleasy.LOGIC.Logics.JoinTables;
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using Microsoft.AspNetCore.Hosting;
using shuttleasy.DAL.Models.dto.Session.dto;
using shuttleasy.DAL.Models.dto.PassengerShuttles.dto;
using shuttleasy.LOGIC.Logics.ShuttleSessions;
using shuttleasy.LOGIC.Logics.GeoPoints;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using System.Collections;
using shuttleasy.LOGIC.Logics.Companies;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.Services.NotifService;
using shuttleasy.DAL.Models.dto.Driver.dto;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private IPassengerLogic _passengerLogic;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IPasswordEncryption _passwordEncryption;
        private readonly IJoinTableLogic _joinTableLogic;
        private readonly IShuttleSessionLogic _shuttleSessionLogic;
        private readonly IGeoPointLogic _geoPointLogic;
        private readonly ICompanyWorkerLogic _companyWorkerLogic;
        private readonly INotificationService _notificationService;
        PassengerString message = new PassengerString();
        List<PassengerShuttleDetailsDto> emptyList = new List<PassengerShuttleDetailsDto>();
        

        public PassengerController(IMapper mapper,IUserService userService,
            IPasswordEncryption passwordEncryption, IPassengerLogic passengerLogic, IJoinTableLogic joinTableLogic,
            IShuttleSessionLogic shuttleSessionLogic, IGeoPointLogic geoPointLogic, ICompanyWorkerLogic companyWorkerLogic,
            INotificationService notificationService)
        {
            _passengerLogic = passengerLogic;
            _mapper = mapper;
            _userService = userService;
            _passwordEncryption = passwordEncryption;
            _joinTableLogic = joinTableLogic;
            _shuttleSessionLogic = shuttleSessionLogic;
            _geoPointLogic = geoPointLogic;
            _companyWorkerLogic = companyWorkerLogic;
            _notificationService = notificationService;
        }
        //  [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]

        [HttpPost]
        public async Task<ActionResult<PassengerInfoDto>> SignUp([FromBody] PassengerRegisterDto passengerRegisterDto)
        {           
            try
            {

                bool isCreated = await _passengerLogic.IsPhoneNumberAndEmailExist(passengerRegisterDto.Email, passengerRegisterDto.PhoneNumber);
                if (!isCreated)
                {
                    Passenger? newPassenger = _userService.SignUp(passengerRegisterDto, Roles.Passenger);
                    if (newPassenger != null)
                    {
                        PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(newPassenger);
                        return Ok(newPassenger);
                    }
                    return BadRequest(Error.NotAdded);
                }
                return BadRequest(Error.FoundEmailOrTelephone);                         
            }  
            catch (Exception ex) { 
                return BadRequest(ex.Message) ; 
            }        
        }
       
        [HttpPost]
        public async Task<ActionResult<PassengerInfoDto>> Login([FromBody] EmailPasswordNotifDto emailPasswordNotifDto)
        {          
            try
            {
                Passenger? passenger = _userService.LoginPassenger(emailPasswordNotifDto.Email, emailPasswordNotifDto.Password);
                if (passenger == null)
                {
                    return BadRequest(Error.NotCorrectEmailAndPassword);               
                }

                if (emailPasswordNotifDto.NotificationToken != null)
                {
                    passenger.NotificationToken = emailPasswordNotifDto.NotificationToken;
                    bool isPassengerUpdated = await _passengerLogic.UpdateAsync(passenger.Id, passenger);
                    if (!isPassengerUpdated)
                    {
                        return BadRequest(Error.NotUpdatedInformation);
                    }
                }
                PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(passenger);
                return Ok(passengerInfoDto);

            }         
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public ActionResult<bool> DeletePassenger([FromBody] EmailPasswordDto emailPasswordDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passengerFromRequestToken = TokenHelper.GetUserFromRequestToken(Request.Headers,_passengerLogic);
                    Passenger? passengerFromEmail = _passengerLogic.GetPassengerWithEmail(emailPasswordDto.Email);

                    if (passengerFromEmail != null && passengerFromRequestToken != null)
                    {
                        if (passengerFromRequestToken.Id == passengerFromEmail.Id)
                        {
                            if (_passwordEncryption.VerifyPasswordHash(passengerFromEmail.PasswordHash, passengerFromEmail.PasswordSalt, emailPasswordDto.Password))
                            {
                                bool isDeleted = _passengerLogic.DeletePassenger(emailPasswordDto.Email);
                                if (isDeleted)
                                {
                                    return Ok(isDeleted);
                                }
                                else
                                {
                                    return BadRequest(Error.NotDeletedPassenger);
                                }
                            }
                            return BadRequest(Error.NotVerifiedPassword);
                        }
                        return BadRequest(Error.ForeignRequest);
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

        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public ActionResult<PassengerInfoDto> UpdatePassenger([FromBody] UserProfileDto userProfileDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passengerFromRequestToken = TokenHelper.GetUserFromRequestToken(Request.Headers,_passengerLogic);
                    if (passengerFromRequestToken != null)
                    {
                        Passenger? updatedPassenger = _userService.UpdatePassengerProfile(passengerFromRequestToken, userProfileDto);
                        if (updatedPassenger != null)
                        {
                            PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(updatedPassenger);
                            return Ok(passengerInfoDto);
                        }
                        return BadRequest(Error.NotUpdatedInformation);
                    }
                    return BadRequest(Error.NotFoundPassenger);

                }

                return Unauthorized(Error.NotMatchedToken);



            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<PassengerInfoDto> GetPassenger([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passenger = _passengerLogic.GetPassengerWithId(idDto.Id);
                    if (passenger != null)
                    {
                        PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(passenger);
                        return Ok(passengerInfoDto);
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
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<ShuttleDto> GetMyShuttleSessions()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    int userId = TokenHelper.GetUserIdFromRequestToken(Request.Headers);
                    var list = _joinTableLogic.OzimYapmaz(userId);
                    if(list.Count != 0)
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

        [HttpPost, Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
        public ActionResult<List<Passenger>> GetAllPassengers()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _passengerLogic.GetAllPassengers();
                    if (list != null)
                    {
                        return list;
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
        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public async Task<ActionResult<bool>> UploadPhoto(IFormFile file)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passenger = TokenHelper.GetUserFromRequestToken(Request.Headers,_passengerLogic);
                    if(passenger != null)
                    {
                        string fileName = passenger.Id + "Image";
                        bool isAdded = await _userService.UploadPhoto(file, fileName);
                        if (isAdded)
                        {
                            return isAdded;
                        }
                        return isAdded;
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
        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public ActionResult<SessionPassenger> GetMySessionPassenger([FromBody] IdDto shuttleId)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    int userId = TokenHelper.GetUserIdFromRequestToken(Request.Headers);
                    SessionPassenger sessionPassenger = _joinTableLogic.GetSessionPassengerJoinTables(userId, shuttleId.Id)[0];
                    return Ok(sessionPassenger);
                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<PassengerShuttleDto> GetRoute([FromBody] IdDto shuttleId)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    PassengerShuttleDto? passengerShuttleDto = _joinTableLogic.GetRouteJoinTables(shuttleId.Id)[0];
                    if(passengerShuttleDto == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    List<PassengerRouteDto> passengerRouteDto = _joinTableLogic.PassengerRouteByPickupOrderJoinTables(shuttleId.Id);
                    if (passengerRouteDto == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(shuttleId.Id);
                    if (shuttleSession == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    double startLong = Double.Parse(_geoPointLogic.Find((int)shuttleSession.StartGeopoint).Longtitude, CultureInfo.InvariantCulture);
                    double startLat = Double.Parse(_geoPointLogic.Find((int)shuttleSession.StartGeopoint).Latitude, CultureInfo.InvariantCulture);
                    double finalLong = Double.Parse(_geoPointLogic.Find((int)shuttleSession.FinalGeopoint).Longtitude, CultureInfo.InvariantCulture);
                    double finalLat = Double.Parse(_geoPointLogic.Find((int)shuttleSession.FinalGeopoint).Latitude, CultureInfo.InvariantCulture);

             

                    List<double[]>? routePoints = new List<double[]>();
                    double[] startGeoPoint = { startLong, startLat };
                    routePoints.Add(startGeoPoint);
                    foreach (PassengerRouteDto passenger in passengerRouteDto)
                    {
                        double longitude = Double.Parse(passenger.Longtitude, CultureInfo.InvariantCulture); 
                        double latitude = Double.Parse(passenger.Latitude, CultureInfo.InvariantCulture);
                        double[] passengerGeoPoints = { longitude, latitude };
                        routePoints.Add(passengerGeoPoints);                 
                    }
                    double[] finalGeoPoint = { finalLong, finalLat };
                    routePoints.Add(finalGeoPoint);
                    passengerShuttleDto.RoutePoints = routePoints;


                    return Ok(passengerShuttleDto);



                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public async Task<ActionResult<bool>> WaitMe([FromBody] IdDto shuttleId )
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(shuttleId.Id);
                    if (shuttleSession == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    CompanyWorker? companyWorker = _companyWorkerLogic.GetCompanyWorkerWithId(shuttleSession.DriverId);
                    if (companyWorker == null)
                    {
                        return BadRequest(Error.NotFound);
                    }
                    NotificationModelToken notificationModelToken = new NotificationModelToken();
                    List<string> tokenList = new List<string>();
                    tokenList.Add(companyWorker.NotificationToken);
                    notificationModelToken.Token = tokenList;
                    notificationModelToken.Title = NotificationTitle.WAIT_ME;
                    notificationModelToken.Body = NotificationBody.WAIT_ME;
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
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<bool> IsNotificationTokenEqual([FromBody] PassengerNotificationTokenDto passengerNotificationTokenDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passenger = _passengerLogic.GetSingle(passengerNotificationTokenDto.Id);
                    if (passenger != null)
                    {
                        if(passengerNotificationTokenDto.NotificationToken == passenger.NotificationToken)
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
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public async Task<ActionResult<bool>> UpdateNotificationToken([FromBody] PassengerNotificationTokenDto passengerNotificationTokenDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passenger = _passengerLogic.GetSingle(passengerNotificationTokenDto.Id);
                    if (passenger != null)
                    {
                        passenger.NotificationToken = passengerNotificationTokenDto.NotificationToken;
                        bool isUpdated = await _passengerLogic.UpdateAsync(passengerNotificationTokenDto.Id, passenger);
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


        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public async Task<ActionResult<Passenger>> UploadImage(IFormFile file)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passenger = TokenHelper.GetUserFromRequestToken(Request.Headers,_passengerLogic);
                    if (passenger == null)
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
                                passenger.ProfilePic = byteArray;
                                bool isPassengerUpdated = await _passengerLogic.UpdateAsync(passenger.Id, passenger);
                                if (isPassengerUpdated)
                                {
                                    return Ok(passenger);
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
        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
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
                        return File(imageData, "image/jpg"); // veya "image/png" veya "image/gif" gibi uygun MIME türünü belirtebilirsiniz

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

        /*
        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public IActionResult DisplayImage()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Passenger? passenger = TokenHelper.GetUserFromRequestToken(Request.Headers, _passengerLogic);
                    if (passenger == null)
                    {
                        return BadRequest(Error.NotFoundPassenger);
                    }
                    if(passenger.ProfilePic != null)
                    {
                        string str = Encoding.UTF8.GetString(passenger.ProfilePic);
                        var imageData = Convert.FromBase64String(str);
                        return File(imageData, "image/jpg"); // veya "image/png" veya "image/gif" gibi uygun MIME türünü belirtebilirsiniz

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
        */
    }
















    /*
     * 
     *  private int GetUserIdFromRequestToken()
    {
        string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
        string user = jwt.Claims.First(c => c.Type == "id").Value;
        int userId = int.Parse(user);
        return userId;
    }
    private bool IsSamePerson(string email)
    {
        string token = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
        Passenger passengerFromToken = _passengerLogic.GetPassengerWithToken(token)
            ?? throw new AuthenticationException();
        Passenger passenger = _passengerLogic.GetPassengerWithEmail(email)
            ?? throw new ArgumentNullException();

        if (passengerFromToken.Id == passenger.Id)
        {
            return true;
        }
        return false;

    }*/


}







