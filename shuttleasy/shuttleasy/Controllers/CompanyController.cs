using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shuttleasy.DAL.Models;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.DAL.Models.dto.Login.dto;
using shuttleasy.Services;
using shuttleasy.LOGIC.Logics.Companies;
using shuttleasy.DAL.Resource.String;
using Microsoft.AspNetCore.Authorization;
using shuttleasy.DAL.Models.dto.GeoPoints.dto;
using shuttleasy.Resource;
using shuttleasy.LOGIC.Logics.JoinTables;
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using shuttleasy.DAL.Models.dto.Companies.dto;
using static shuttleasy.LOGIC.Logics.JoinTables.JoinTableLogic;

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
        private readonly IJoinTableLogic _joinTableLogic;

        public CompanyController(IUserService userService, IPassengerLogic passengerLogic, ICompanyWorkerLogic driverLogic,
            IMapper mapper,ICompanyLogic companyLogic, IJoinTableLogic joinTableLogic)
        {
            _userService = userService;
            _passengerLogic = passengerLogic;
            _driverLogic = driverLogic;
            _mapper = mapper;
            _companyLogic = companyLogic;
            _joinTableLogic = joinTableLogic;
        }
        [HttpPost, Authorize(Roles = $"{Roles.Admin}")]
        public ActionResult<bool> AddCompany([FromBody] CompanyDto companyDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    Company company = _mapper.Map<Company>(companyDto);
                    company.VotesNumber = 0;
                    bool isAdded =  _companyLogic.Add(company);
                    if (isAdded)
                    {
                        return Ok(isAdded);
                        /*
                        GeoPoint addedGeoPoint = await _geoPointLogic.GetGeoPointWithLocationName(geoPointDto.LocationName);
                        int idNumber = addedGeoPoint.Id;
                        if (idNumber != null)
                        {
                            return Ok(idNumber);
                        }
                        return BadRequest(Error.EmptyList);
                        */
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
        public ActionResult<bool> DeleteCompany([FromBody] IdDto idDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    bool isAdded = _companyLogic.Delete(idDto.Id);
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
        public ActionResult<string> GetCompanyName([FromBody] IdDto idDto)
        {

            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    string? companyName = _companyLogic.GetCompanyNameWithCompanyId(idDto.Id);
                    if (companyName != null)
                    {
                        return Ok(companyName);
                    }
                    return BadRequest(Error.NotFoundCompany);

                }
                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /*
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<ShuttleDetailsDto> GetShuttleDetails([FromBody] IdDto companyId)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var details = _joinTableLogic.ShuttleDetailsInnerJoinTables(companyId.Id);
                    if (details != null)
                    {
                        return Ok(details);
                    }

                    return BadRequest(Error.EmptyList);
                }

                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        */
        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public async Task<ActionResult<bool>> EditCompany([FromBody] EditCompanyDto editCompanyDto)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    int id = editCompanyDto.Id;
                    Company? company = _companyLogic.Find(id);
                    if(company != null)
                    {
                        Company updatedCompany = _mapper.Map<Company>(editCompanyDto);
                        updatedCompany.Rating = company.Rating;
                        bool isUpdated = await _companyLogic.UpdateAsync(id, updatedCompany);
                        if (isUpdated)
                        {
                            return Ok(isUpdated);
                        }
                        return BadRequest(isUpdated);
                    }
                    return BadRequest(Error.NotFoundCompany);
                   

                }
                return Unauthorized(Error.NotMatchedToken);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = $"{Roles.Passenger},{Roles.Driver},{Roles.Admin}")]
        public ActionResult<CompanyDetailGroupDto> GetCompany([FromBody] IdDto companyId)
        {
            try
            {
                UserVerifyingDto userInformation = TokenHelper.GetUserInformation(Request.Headers);
                if (_userService.VerifyUser(userInformation))
                {
                    var list = _joinTableLogic.CompanyDetailsInnerJoinTables(companyId.Id);
                    if(list.Capacity != 0)
                    {
                        return Ok(list);
                    }
                    return BadRequest(Error.EmptyList);
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
