using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Login.dto;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.Companies;
using shuttleasy.DAL.Resource.String;

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

        public CompanyController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
            IMapper mapper,ICompanyLogic companyLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
            _companyLogic = companyLogic;
        }
        [HttpPost]
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
       
    }
}
