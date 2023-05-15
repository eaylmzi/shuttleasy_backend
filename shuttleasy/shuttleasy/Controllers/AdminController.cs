using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.DAL.Models.dto.Login.dto;
using shuttleasy.DAL.Models.dto.Passengers.dto;
using shuttleasy.Resource;
using shuttleasy.Services;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using shuttleasy.Route;
using shuttleasy.DAL.Models.dto.Companies.dto;
using shuttleasy.LOGIC.Logics.JoinTables;
using shuttleasy.DAL.Models.dto.PassengerRatingDto;
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using shuttleasy.DAL.Models.dto.SessionPassengers.dto;
using shuttleasy.LOGIC.Logics.SessionPassengers;
using System.Text;
using shuttleasy.DAL.Models.dto.Image.dto;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IMapper _mapper;
        private readonly IJoinTableLogic _joinTableLogic;
        private readonly ISessionPassengerLogic _sessionPassengerLogic;
        List<CommentDetailsDto> emptyList = new List<CommentDetailsDto>();
        public AdminController(IUserService userService, IPassengerLogic passengerLogic ,ICompanyWorkerLogic driverLogic,
            IMapper mapper, IJoinTableLogic joinTableLogic, ISessionPassengerLogic sessionPassengerLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
            _joinTableLogic = joinTableLogic;
            _sessionPassengerLogic = sessionPassengerLogic;
        }
        [HttpPost]
        public ActionResult<CompanyWorkerInfoDto> Login([FromBody]EmailPasswordDto emailPasswordDto)
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
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public async Task<ActionResult<CompanyWorkerInfoDto>> CreateDriver([FromBody]CompanyWorkerRegisterDto driverRegisterDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    bool isCreated = await _driverLogic.IsPhoneNumberAndEmailExist(driverRegisterDto.Email, driverRegisterDto.PhoneNumber);
                    if (!isCreated)
                    {
                        CompanyWorker? newCompanyWorker = _userService.CreateCompanyWorker(driverRegisterDto, Roles.Driver);
                        if (newCompanyWorker != null)
                        {
                            CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(newCompanyWorker);
                            return Ok(driverInfoDto);
                        }
                        return BadRequest(Error.NotAdded);
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
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public async Task<ActionResult<PassengerInfoDto>> CreatePassenger([FromBody] PassengerRegisterPanelDto passengerRegisterPanelDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    bool isCreated = await _passengerLogic.IsPhoneNumberAndEmailExist(passengerRegisterPanelDto.Email, passengerRegisterPanelDto.PhoneNumber);
                    if (!isCreated)
                    {
                        Passenger? newPassenger = _userService.CreatePassenger(passengerRegisterPanelDto, Roles.Passenger);
                        if (newPassenger != null)
                        {
                            PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(newPassenger);
                            return Ok(passengerInfoDto);
                        }
                        return BadRequest(Error.NotAdded);

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
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<CommentDetailsDto> GetAllComment()
        {          
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = TokenHelper.GetCompanyWorkerFromRequestToken(Request.Headers, _driverLogic);
                    if (companyWorker != null)
                    {
                        var list = _joinTableLogic.CommentDetailsInnerJoinTables(companyWorker.CompanyId);
                        if (list.Count != 0)
                        {
                            return Ok(list);
                        }
                        return Ok(emptyList);
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
        public ActionResult<EnrolledPassengersGroupDto> GetAllRegisteredPassengers()
        {           
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = TokenHelper.GetCompanyWorkerFromRequestToken(Request.Headers, _driverLogic);
                    if (companyWorker != null)
                    {
                        var list = _joinTableLogic.ShuttlePassengersInnerJoinTables(companyWorker.CompanyId);
                        if (list.Count != 0)
                        {
                            return Ok(list);
                        }
                        return Ok(emptyList);

                    }
                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<DriversInfoDto> GetDriversStatisticByRatingAvg()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = TokenHelper.GetCompanyWorkerFromRequestToken(Request.Headers, _driverLogic);
                    if (companyWorker != null)
                    {
                        var list = _joinTableLogic.CompanyWorkerDriverStaticticRatingAvgJoinTables(companyWorker.CompanyId);
                        if (list.Count != 0)
                        {
                            return Ok(list);
                        }
                        return Ok(emptyList);
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
        public ActionResult<DriversInfoDto> GetDriversStatisticByWorkingHours()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = TokenHelper.GetCompanyWorkerFromRequestToken(Request.Headers, _driverLogic);
                    if (companyWorker != null)
                    {
                        var list = _joinTableLogic.CompanyWorkerDriverStaticticWorkingHoursJoinTables(companyWorker.CompanyId);
                        if (list.Count != 0)
                        {
                            return Ok(list);
                        }
                        return Ok(emptyList);
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
        public ActionResult<DriversInfoDto> GetDriversStatisticByName()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = TokenHelper.GetCompanyWorkerFromRequestToken(Request.Headers, _driverLogic);
                    if (companyWorker != null)
                    {
                        var list = _joinTableLogic.CompanyWorkerDriverStaticticByNameJoinTables(companyWorker.CompanyId);
                        if (list.Count != 0)
                        {
                            return Ok(list);
                        }
                        return Ok(emptyList);
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
        public ActionResult<bool> DeleteSessionPassenger([FromBody] ShuttleAndPassengerIdDto shuttleAndPassengerIdDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    SessionPassenger sessionPassenger = _joinTableLogic.GetSessionPassengerJoinTables(shuttleAndPassengerIdDto.PassengerId, shuttleAndPassengerIdDto.ShuttleId)[0];
                    bool isDeleted = _sessionPassengerLogic.Delete(sessionPassenger.Id);
                    if (isDeleted)
                    {
                        return Ok(isDeleted);
                    }
                    return BadRequest(isDeleted);


                }
                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public async Task<ActionResult<CompanyWorker>> UploadCompanyWorkerImage(ImageIdDto file)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(file.Id);
                    if (companyWorker == null)
                    {
                        return BadRequest(Error.NotFoundPassenger);
                    }

                    if (file != null && file.File.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var extension = Path.GetExtension(file.File.FileName);
                        if (!allowedExtensions.Contains(extension.ToLower()))
                        {
                            return BadRequest("Invalid file type. Only JPG, JPEG and PNG files are allowed.");
                        }

                        try
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.File.CopyTo(ms);
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
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public IActionResult DisplayCompanyWorkerImage([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(idDto.Id);
                    if (companyWorker == null)
                    {
                        return BadRequest(Error.NotFoundPassenger);
                    }
                    if (companyWorker.ProfilePic != null)
                    {
                        string str = Encoding.UTF8.GetString(companyWorker.ProfilePic);
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
         * Yedek tokenhelper
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
                    userVerifyingDto.Id = Token.GetUserIdFromRequestToken(Request.Headers);
                    userVerifyingDto.Token = GetUserTokenFromRequestToken();
                    userVerifyingDto.Role = GetUserRoleFromRequestToken();
                    return userVerifyingDto;
                }
                private CompanyWorker? GetAdminFromRequestToken()
                {
                    string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
                    CompanyWorker? adminFromToken = _driverLogic.GetCompanyWorkerWithToken(requestToken);
                    return adminFromToken;
                }

        */

        /*

        [HttpPost]
        public IActionResult Test1()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "shuttleasydbserver1.database.windows.net";
                builder.UserID = "emreyilmaz";
                builder.Password = "Easypeasy1";
                builder.InitialCatalog = "ShuttleasyDB";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {


                    connection.Open();

                    string sql = "SELECT name  FROM company_worker";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return Ok(reader.GetString(0));
                            }
                        }
                    }

                }
                return Ok();
            }
            catch (SqlException e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public IActionResult Test2()
        {
            try
            {
                return Ok(GetUserRoleFromRequestToken());
            }
            catch (SqlException e)
            {
                return BadRequest(e.Message);
            }

        }
        */
    }
}
