﻿using AutoMapper;
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
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.Models.dto.Driver.dto;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

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
                CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken());
                if (companyWorker != null)
                {
                    ShuttleBus shuttleBus = _mapper.Map<ShuttleBus>(shuttleBusDto);
                    bool isAdded = _shuttleBusLogic.Add(shuttleBus);
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
        [HttpPost, Authorize(Roles = $"{Roles.Driver},{Roles.Admin},{Roles.SuperAdmin}")]
        public ActionResult<bool> DeleteShuttleBus([FromBody] IdDto idDto)
        {
            try
            {
                CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(GetUserIdFromRequestToken());
                if (companyWorker != null)
                {
                    bool isAdded = _shuttleBusLogic.DeleteShuttleBus(idDto.Id);
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