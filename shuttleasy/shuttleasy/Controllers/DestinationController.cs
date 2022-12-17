using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.Encryption;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.Login.dto;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.Destinations;
using shuttleasy.Models.dto.Destinations.dto;
using shuttleasy.Models.dto.Passengers.dto;
using AutoMapper;
using shuttleasy.DAL.Resource.String;
using Microsoft.AspNetCore.Authorization;
using shuttleasy.Models.dto.Credentials.dto;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DestinationController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IDestinationLogic _destinationLogic;
        private readonly IMapper _mapper;

        public DestinationController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
                    IDestinationLogic destinationLogic, IMapper mapper)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _destinationLogic = destinationLogic;
            _mapper = mapper;
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<bool> AddDestination([FromBody] DestinationDto destinationDto)
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    Destination destination = _mapper.Map<Destination>(destinationDto);
                    bool isAdded = _destinationLogic.Add(destination);
                    if (isAdded)
                    {
                        return Ok(isAdded);
                    }
                    return BadRequest(isAdded);

                }
                return BadRequest("The user that send request not found");
               
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
           

           
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<bool> DeleteDestination([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    bool isAdded = _destinationLogic.DeleteDestination(idDto.Id);
                    if (isAdded)
                    {
                        return Ok(isAdded);
                    }
                    return BadRequest(isAdded);


                }
                return BadRequest("The user that send request not found");
              
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<List<Destination>> GetAllDestinations()
        {
            try
            {
                UserVerifyingDto userInformation = GetUserInformation();
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _destinationLogic.GetAllDestinations();
                    if (list != null)
                    {
                        return list;
                    }
                    return BadRequest("There is no destination in list");
                }
                return BadRequest("Mistake about token");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
            userVerifyingDto.Id = GetUserIdFromRequestToken();
            userVerifyingDto.Token = GetUserTokenFromRequestToken();
            userVerifyingDto.Role = GetUserRoleFromRequestToken();
            return userVerifyingDto;
        }
    }
}
