﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.GeoPoints.dto;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.GeoPoints;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Resource;
using shuttleasy.Services;
using shuttleasy.DAL.EFRepositories.PickupAreas;
using shuttleasy.LOGIC.Logics.PickupAreas;
using shuttleasy.DAL.Models.dto.PickupArea.dto;
using System.ComponentModel.Design;
using shuttleasy.LOGIC.Logics.JoinTables;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PickupAreaController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IPickupAreaLogic _pickupAreaLogic;
        private readonly IJoinTableLogic _joinTableLogic;
        private readonly IMapper _mapper;
        public PickupAreaController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic, IPickupAreaLogic pickupAreaLogic,
           IMapper mapper, IJoinTableLogic joinTableLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _pickupAreaLogic = pickupAreaLogic;
            _mapper = mapper;
            _joinTableLogic = joinTableLogic;
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<bool> AddPickupArea([FromBody] PickupAreaDto pickupAreaDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    PickupArea pickupArea = _mapper.Map<PickupArea>(pickupAreaDto);
                    bool isAdded = _pickupAreaLogic.Add(pickupArea);
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
        public ActionResult<bool> DeletePickupArea([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    bool isAdded = _pickupAreaLogic.Delete(idDto.Id);
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
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Admin}")]
        public ActionResult<bool> GetShuttlePickUpArea([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    PickupArea? isAdded = _pickupAreaLogic.Find(idDto.Id);
                    if (isAdded != null)
                    {
                        return Ok(isAdded);
                    }
                    return Ok(isAdded);
                }
                return Unauthorized(Error.NotMatchedToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Admin}")]
        public ActionResult<List<PickupArea>> GetShuttlePickUpAreas([FromBody] ListIdDto listIdDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _joinTableLogic.ShuttlePickUpAreaInnerJoinTables(listIdDto.IdList);
                    if (list != null)
                    {
                        return Ok(list);
                    }
                    return Ok(list);
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
