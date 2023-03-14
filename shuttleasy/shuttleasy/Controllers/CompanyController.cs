﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.DAL.Models.dto.Login.dto;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.Companies;
using shuttleasy.DAL.Resource.String;
using Microsoft.AspNetCore.Authorization;
using shuttleasy.DAL.Models.dto.GeoPoints.dto;
using shuttleasy.Resource;
using shuttleasy.DAL.Models.dto.ShuttleDetails.dto;
using shuttleasy.LOGIC.Logics.ShuttleDetails;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompanyController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IMapper _mapper;
        private readonly ICompanyLogic _companyLogic;
        private readonly IShuttleDetailsLogic _shuttleDetailsLogic;

        public CompanyController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
            IMapper mapper,ICompanyLogic companyLogic, IShuttleDetailsLogic shuttleDetailsLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
            _companyLogic = companyLogic;
            _shuttleDetailsLogic = shuttleDetailsLogic;
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<bool> AddCompany([FromBody] Company companyDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Company company = _mapper.Map<Company>(companyDto);
                    bool isAdded =  _companyLogic.Add(company);
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
        public ActionResult<bool> DeleteCompany([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    bool isAdded = _companyLogic.Delete(idDto.Id);
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
        public ActionResult<string> GetCompanyName([FromBody] IdDto idDto)
        {
            try
            {
                string? companyName = _companyLogic.GetCompanyNameWithCompanyId(idDto.Id);
                if(companyName != null)
                {
                    return Ok(companyName);
                }
                return BadRequest(Error.NotFoundCompany);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<ShuttleDetailsDto> GetShuttleDetails([FromBody] IdDto companyId)
        {
            try
            {
                
                var details = _shuttleDetailsLogic.InnerJoinTables(companyId.Id);
                if(details != null)
                {
                    return Ok(details);
                }

                return BadRequest(Error.EmptyList);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
