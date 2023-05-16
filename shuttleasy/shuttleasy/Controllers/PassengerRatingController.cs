using AutoMapper;
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
using shuttleasy.LOGIC.Logics.SessionHistories;
using shuttleasy.LOGIC.Logics.DriversStatistics;

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
        private readonly ISessionHistoryLogic _sessionHistoryLogic;
        private readonly IDriversStatisticLogic _driversStatisticLogic;
        List<PassengerRating> emptyList = new List<PassengerRating>();
        public PassengerRatingController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
            IPassengerRatingLogic passengerRatingLogic, IShuttleSessionLogic shuttleSessionLogic, ICompanyLogic companyLogic,
           IMapper mapper, IJoinTableLogic joinTableLogic, ISessionHistoryLogic sessionHistoryLogic, IDriversStatisticLogic driversStatisticLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _passengerRatingLogic = passengerRatingLogic;
            _shuttleSessionLogic = shuttleSessionLogic;
            _companyLogic = companyLogic;
            _mapper = mapper;
            _joinTableLogic = joinTableLogic;
            _sessionHistoryLogic = sessionHistoryLogic;
            _driversStatisticLogic = driversStatisticLogic;
        }

        [HttpPost, Authorize(Roles = $"{Roles.Passenger}")]
        public async Task<ActionResult<bool>> Comment([FromBody] CommentDto commentDto)
        {
            UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
            if (_userService.VerifyUser(userInformation))
            {
                try
                {                
                    if (_passengerRatingLogic.GetSingle(userInformation.Id,commentDto.SessionId) == null)
                    {

                        SessionHistory sessionHistory = _sessionHistoryLogic.GetSingleBySessionId(commentDto.SessionId);
                        if (sessionHistory == null)
                        {
                            return BadRequest("sessionhistory");
                        }
                        bool isSessionHistoryUpdated = await _userService.UpdateSessionHistoryRating(sessionHistory, commentDto.Rating);
                        if (!isSessionHistoryUpdated)
                        {
                            return BadRequest("sessionhistory not updated");
                        }
                        ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(commentDto.SessionId);
                        if (shuttleSession == null)
                        {
                            return BadRequest("shuttlesession");
                        }
                        DriversStatistic? driverStatictic = _driversStatisticLogic.GetSingleDriverId(shuttleSession.DriverId);
                        if (driverStatictic == null)
                        {
                            return BadRequest("driverstatictic");
                        }
                        bool isDriverStatisticUpdated = await _userService.UpdateDriverStaticticRating(driverStatictic, commentDto.Rating, shuttleSession.DriverId);

                        if (!isDriverStatisticUpdated)
                        {
                            return BadRequest("driverstatictic not updated");
                        }

                        PassengerRating passengerRating = _mapper.Map<PassengerRating>(commentDto);
                        passengerRating.PassengerIdentity = TokenHelper.GetUserIdFromRequestToken(Request.Headers);
                        passengerRating.Date = DateTime.Now;
                        bool isAdded = _passengerRatingLogic.Add(passengerRating);
                        if (isAdded)
                        {
                            bool isUpdated = _userService.UpdateCompanyRating(commentDto.SessionId, commentDto.Rating);
                            if (isUpdated)
                            {
                                return BadRequest(isUpdated);
                            }
                            return BadRequest(isUpdated);
                        }
                        return BadRequest(isAdded);

                       
                       

                    }
                    return BadRequest(Error.AlreadyComment);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            return Unauthorized(Error.NotMatchedToken);

        }
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Admin}")]
        public ActionResult<CommentDetailsDto> GetComment([FromBody] IdDto companyIdDto)
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
