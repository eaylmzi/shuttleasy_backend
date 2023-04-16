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
        List<CommentDetailsDto> emptyList = new List<CommentDetailsDto>();
        public AdminController(IUserService userService, IPassengerLogic passengerLogic ,ICompanyWorkerLogic driverLogic,
            IMapper mapper, IJoinTableLogic joinTableLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
            _joinTableLogic = joinTableLogic;
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
            UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
            if (_userService.VerifyUser(userInformation))
            {
               
                try
                {
                    CompanyWorker? companyWorker = TokenHelper.GetCompanyWorkerFromRequestToken(Request.Headers, _driverLogic);
                    if(companyWorker != null)
                    {
                        var list = _joinTableLogic.CommentDetailsInnerJoinTables(companyWorker.CompanyId);
                        if (list.Count != 0)
                        {
                            return Ok(list);
                        }
                        return Ok(emptyList);

                    }
                    



                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            return Unauthorized(Error.NotMatchedToken);
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<EnrolledPassengersGroupDto> GetAllRegisteredPassengers()
        {
            UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
            if (_userService.VerifyUser(userInformation))
            {

                try
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
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            return Unauthorized(Error.NotMatchedToken);
        }

        [HttpPost]
        public ActionResult Location()
        {
            Location loc1 = new Location()
            {
                Longitude = "0",
                Latitude = "0"
            };
            Location loc2 = new Location()
            {
                Longitude = "0",
                Latitude = "4"
            };

            Routes route = new Routes();
            


            return Ok(route.calculateDistance(loc1, loc2));
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
