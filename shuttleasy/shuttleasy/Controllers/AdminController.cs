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
        public AdminController(IUserService userService, IPassengerLogic passengerLogic ,ICompanyWorkerLogic driverLogic,
            IMapper mapper)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
        }
        [HttpPost]
        public ActionResult<CompanyWorkerInfoDto> Login([FromBody]EmailPasswordDto emailPasswordDto)
        {
            try
            {
                bool isLogin = _userService.LoginCompanyWorker(emailPasswordDto.Email, emailPasswordDto.Password);
                if (isLogin)
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithEmail(emailPasswordDto.Email);
                    if (companyWorker != null)
                    {
                        CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(companyWorker);
                        return Ok(driverInfoDto);
                    }
                    return BadRequest(Error.NotFoundAdmin);
                    
                }
                return BadRequest(Error.NotCorrectEmailAndPassword);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<CompanyWorkerInfoDto> CreateDriver([FromBody]CompanyWorkerRegisterDto driverRegisterDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    bool isCreated = _userService.CheckEmailandPhoneNumberForCompanyWorker(driverRegisterDto.Email,driverRegisterDto.PhoneNumber);
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
        public ActionResult<PassengerInfoDto> CreatePassenger([FromBody] PassengerRegisterPanelDto passengerRegisterPanelDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    bool isCreated = _userService.CheckEmailandPhoneNumberForPassengers(passengerRegisterPanelDto.Email,passengerRegisterPanelDto.PhoneNumber);
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
