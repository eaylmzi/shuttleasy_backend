using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.GeoPoints;
using Microsoft.AspNetCore.Authorization;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Resource;
using shuttleasy.DAL.Models.dto.GeoPoints.dto;
using Microsoft.EntityFrameworkCore;
using shuttleasy.LOGIC.Logics.PickupAreas;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GeoPointController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IGeoPointLogic _geoPointLogic;
        private readonly IMapper _mapper;
        private readonly IPickupAreaLogic _pickupAreaLogic;
        List<GeoPoint> emptyList = new List<GeoPoint>();
        public GeoPointController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,IGeoPointLogic geoPointLogic,
           IMapper mapper, IPickupAreaLogic pickupAreaLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _geoPointLogic = geoPointLogic; 
            _mapper = mapper;
            _pickupAreaLogic = pickupAreaLogic;
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public async Task<ActionResult<bool>> AddGeoPoint([FromBody] GeoPointDto geoPointDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    GeoPoint geoPoint = _mapper.Map<GeoPoint>(geoPointDto);
                    GeoPoint? isAddedGeoPoint = _geoPointLogic.FindByCoordinateGeoPoint(geoPoint.Longtitude, geoPoint.Latitude);
                    if(isAddedGeoPoint != null)
                    {
                        return Ok(isAddedGeoPoint);
                    }
                    bool isAdded = await _geoPointLogic.AddAsync(geoPoint);
                    if (isAdded)
                    {
                        return Ok(isAdded);
                        /*
                        GeoPoint addedGeoPoint = await _geoPointLogic.GetGeoPointWithLocationName(geoPointDto.LocationName);
                        int idNumber = addedGeoPoint.Id;
                        if (idNumber != null)
                        {
                            return Ok(idNumber);
                        }
                        return BadRequest(Error.EmptyList);
                        */
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
        public ActionResult<bool> DeleteGeoPoint([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    bool isAdded = _geoPointLogic.Delete(idDto.Id);
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

        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver}{Roles.Admin}")]
        public ActionResult<List<GeoPoint>> GetAllGeoPoint()
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                   var list = _geoPointLogic.GetAll();
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
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<GeoPoint> GetGeoPoint([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    GeoPoint? geoPoint = _geoPointLogic.Find(idDto.Id);
                    if(geoPoint != null)
                    {
                        return Ok(geoPoint);
                    }
                    return Ok(geoPoint);
                }
                return Unauthorized(Error.NotMatchedToken);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<string> Lalalalalal()
        {
            return Ok("lalalalalal");
        }
    }
}
