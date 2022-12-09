using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using shuttleasy.DAL.Models;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Passengers.dto;
using shuttleasy.Models.dto.User.dto;

namespace shuttleasy.Models.dto
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {          
            CreateMap<PassengerRegisterDto, Passenger>();
            CreateMap<DriverRegisterDto, CompanyWorker>();
            CreateMap<PassengerRegisterPanelDto, Passenger>();
            CreateMap<UserProfileDto, Passenger>();
            CreateMap<DriverProfileDto, CompanyWorker>();
        }
    }
}
