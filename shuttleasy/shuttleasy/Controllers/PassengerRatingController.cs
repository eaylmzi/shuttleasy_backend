﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.PassengerPayments;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.PassengerRatings;
using shuttleasy.DAL.Models.dto.Passengers.dto;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using Microsoft.AspNetCore.Authorization;
using shuttleasy.DAL.Models.dto.PassengerRatingDto;
using shuttleasy.DAL.Models.dto.ShuttleBuses.dto;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.SessionPassengers.dto;
using shuttleasy.Resource;
using shuttleasy.LOGIC.Logics.ShuttleSessions;
using shuttleasy.LOGIC.Logics.Companies;
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using shuttleasy.LOGIC.Logics.JoinTables;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PassengerRatingController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IPassengerRatingLogic _passengerRatingLogic;
        private readonly IMapper _mapper;
        private readonly IShuttleSessionLogic _shuttleSessionLogic;
        private readonly ICompanyLogic _companyLogic;
        private readonly IJoinTableLogic _joinTableLogic;
        List<PassengerRating> emptyList = new List<PassengerRating>();
        public PassengerRatingController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
            IPassengerRatingLogic passengerRatingLogic, IShuttleSessionLogic shuttleSessionLogic, ICompanyLogic companyLogic,
           IMapper mapper, IJoinTableLogic joinTableLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _passengerRatingLogic = passengerRatingLogic;
            _shuttleSessionLogic = shuttleSessionLogic;
            _companyLogic = companyLogic;
            _mapper = mapper;
            _joinTableLogic = joinTableLogic;
        }

        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public ActionResult<bool> Comment([FromBody] CommentDto commentDto)
        {
            UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
            if (_userService.VerifyUser(userInformation))
            {
                try
                {
                    PassengerRating passengerRating = _mapper.Map<PassengerRating>(commentDto);
                    passengerRating.PassengerIdentity = TokenHelper.GetUserIdFromRequestToken(Request.Headers);
                    passengerRating.Date = DateTime.Now;
                    bool isAdded = _passengerRatingLogic.Add(passengerRating);
                    if (isAdded)
                    {
                        bool isUpdated = _userService.calculateRating(commentDto.SessionId, commentDto.Rating);
                        if (isUpdated)
                        {
                            return Ok(isUpdated);
                        }
                        return BadRequest(isUpdated);
                    }
                    return BadRequest(isAdded);




                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            return Unauthorized(Error.NotMatchedToken);

        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Admin}")]
        public ActionResult<bool> GetComment([FromBody] IdDto companyIdDto)
        {
            UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
            if (_userService.VerifyUser(userInformation))
            {
                try
                {
                    var list = _joinTableLogic.CommentDetailsInnerJoinTables(companyIdDto.Id);
                    if(list.Count != 0)
                    {
                        return Ok(list);
                    }                  
                    return Ok(emptyList);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            return Unauthorized(Error.NotMatchedToken);

        }

    }
}
