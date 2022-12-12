using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using shuttleasy.DAL.Models;
using shuttleasy.Models.dto.Destinations.dto;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Passengers.dto;
using shuttleasy.Models.dto.ShuttleBuses.dto;
using shuttleasy.Models.dto.ShuttleSessions.dto;
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
            CreateMap<DestinationDto, Destination>();
            CreateMap<ShuttleBusDto, ShuttleBus>();
            CreateMap<ShuttleSessionDto, ShuttleSession>();

            CreateMap<CompanyWorker, DriverInfoDto>();
            CreateMap<Passenger, PassengerInfoDto>();
        }
    }
}
