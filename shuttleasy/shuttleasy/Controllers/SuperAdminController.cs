using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Services;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using shuttleasy.Models.dto.Login.dto;

namespace shuttleasy.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SuperAdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IMapper _mapper;
        public SuperAdminController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
            IMapper mapper)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
        }
        [HttpPost]
        public ActionResult<CompanyWorkerInfoDto> Login([FromBody] EmailPasswordDto emailPasswordDto)
        {
            try
            {
                bool isLogin = _userService.LoginCompanyWorker(emailPasswordDto.Email, emailPasswordDto.Password);
                if (isLogin)
                {
                    CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithEmail(emailPasswordDto.Email);
                    if (companyWorker != null)
                    {
                        CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(companyWorker);
                        return Ok(driverInfoDto);
                    }
                    return BadRequest("The Superadmin not found in list");

                }
                return BadRequest("Email and password not correct");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost, Authorize(Roles = $"{Roles.SuperAdmin}")]
        public ActionResult<CompanyWorkerInfoDto> CreateAdmin([FromBody] CompanyWorkerRegisterDto adminRegisterDto)
        {
            try
            {
                CompanyWorker newCompanyWorker = _userService.CreateCompanyWorker(adminRegisterDto, Roles.Admin);
                if (newCompanyWorker != null)
                {
                    CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(newCompanyWorker);
                    return Ok(driverInfoDto);
                }
                return BadRequest("Not Added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
    }
}
