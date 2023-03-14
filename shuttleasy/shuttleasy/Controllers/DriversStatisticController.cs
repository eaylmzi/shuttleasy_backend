using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.GeoPoints;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.DriversStatistics;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DriversStatisticController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IDriversStatisticLogic _driversStatisticLogic;
        private readonly IMapper _mapper;
        public DriversStatisticController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic, IDriversStatisticLogic driversStatisticLogic,
           IMapper mapper)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _driversStatisticLogic = driversStatisticLogic;
            _mapper = mapper;
        }

     
    }
}
