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
using shuttleasy.Resource;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DestinationController : Controller
    {
        private readonly IUserService _userService;
        private readonly IDestinationLogic _destinationLogic;
        private readonly IMapper _mapper;

        public DestinationController(IUserService userService,
                    IDestinationLogic destinationLogic, IMapper mapper)
        {
            _userService = userService;
            _destinationLogic = destinationLogic;
            _mapper = mapper;
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<bool> AddDestination([FromBody] DestinationDto destinationDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
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
                return Unauthorized(Error.NotMatchedToken);
               
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
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    bool isAdded = _destinationLogic.DeleteDestination(idDto.Id);
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
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<List<Destination>> GetAllDestinations()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _destinationLogic.GetAllDestinations();
                    if (list != null)
                    {
                        return list;
                    }
                    return BadRequest(Error.NotFoundDestination);
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
