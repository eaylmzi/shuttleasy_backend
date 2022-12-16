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
        [HttpPost]
        public ActionResult<bool> AddDestination([FromBody] DestinationDto destinationDto)
        {
            try
            {
                CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken());
                if (companyWorker != null)
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
        [HttpPost]
        public ActionResult<bool> DeleteDestination([FromBody] IdDto idDto)
        {
            try
            {
                CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken());
                if (companyWorker != null)
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

        private int GetUserIdFromRequestToken()
        {
            string requestToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
            string user = jwt.Claims.First(c => c.Type == "id").Value;
            int userId = int.Parse(user);
            return userId;
        }
    }
}
