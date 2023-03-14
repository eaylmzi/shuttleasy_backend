using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.SessionHistories;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.SessionPassengers;

namespace shuttleasy.Controllers
{
    public class SessionPassengerController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly ISessionPassengerLogic _sessionPassengerLogic;
        private readonly IMapper _mapper;
        public SessionPassengerController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
            ISessionPassengerLogic sessionPassengerLogic,
           IMapper mapper)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _sessionPassengerLogic = sessionPassengerLogic;
            _mapper = mapper;
        }
    }
}
