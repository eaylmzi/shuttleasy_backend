using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.NotificationPassengers;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.NotificationWorkers;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationWorkerController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly INotificationWorkerLogic _notificationWorkerLogic;
        private readonly IMapper _mapper;
        public NotificationWorkerController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic, INotificationWorkerLogic notificationWorkerLogic,
           IMapper mapper)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _notificationWorkerLogic = notificationWorkerLogic;
            _mapper = mapper;
        }
    }
}
