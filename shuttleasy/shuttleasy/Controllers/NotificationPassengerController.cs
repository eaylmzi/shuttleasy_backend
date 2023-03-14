using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.GeoPoints;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.NotificationPassengers;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationPassengerController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly INotificationPassengerLogic _notificationPassengerLogic;
        private readonly IMapper _mapper;
        public NotificationPassengerController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic, INotificationPassengerLogic notificationPassengerLogic,
           IMapper mapper)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _notificationPassengerLogic = notificationPassengerLogic;
            _mapper = mapper;
        }
    }
}
