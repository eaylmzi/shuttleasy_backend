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
        [HttpPost,Authorize(Roles = $"{Roles.Driver},{Roles.Admin}")]
        public ActionResult<CompanyWorker> AddDestination([FromBody] DestinationDto destinationDto)
        {
            try
            {
                Destination destination = _mapper.Map<Destination>(destinationDto);
                bool isAdded = _destinationLogic.Add(destination);
                if (isAdded)
                {
                    return Ok(destination);
                }
                return BadRequest("Destination not added");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
           

           
        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin}")]
        public ActionResult<CompanyWorker> DeleteDestination([FromBody] int destinationNumber)
        {
            try
            {
                bool isAdded = _destinationLogic.DeleteDestination(destinationNumber);
                if (isAdded)
                {
                    return Ok(isAdded);
                }
                return BadRequest("Destination not deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
     
    }
}
