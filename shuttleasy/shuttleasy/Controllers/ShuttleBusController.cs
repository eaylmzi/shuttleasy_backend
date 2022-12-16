using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.Destinations;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.Destinations.dto;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.ShuttleBuses;
using shuttleasy.Models.dto.ShuttleBuses.dto;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShuttleBusController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IShuttleBusLogic _shuttleBusLogic;
        private readonly IMapper _mapper;

        public ShuttleBusController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
                    IShuttleBusLogic shuttleBusLogic, IMapper mapper)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _shuttleBusLogic = shuttleBusLogic;
            _mapper = mapper;
        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public ActionResult<bool> AddShuttleBus([FromBody] ShuttleBusDto shuttleBusDto)
        {
            try
            {
                ShuttleBus shuttleBus = _mapper.Map<ShuttleBus>(shuttleBusDto);
                bool isAdded = _shuttleBusLogic.Add(shuttleBus);
                if (isAdded)
                {
                    return Ok(isAdded);
                }
                return BadRequest(isAdded);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public ActionResult<bool> DeleteShuttleBus([FromBody] int shuttleBusNumber)
        {
            try
            {
                bool isAdded = _shuttleBusLogic.DeleteShuttleBus(shuttleBusNumber);
                if (isAdded)
                {
                    return Ok(isAdded);
                }
                return BadRequest(isAdded);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
