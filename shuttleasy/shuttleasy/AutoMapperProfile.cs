using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.Passengers.dto;
using shuttleasy.DAL.Models.dto.Destinations.dto;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.DAL.Models.dto.ShuttleBuses.dto;
using shuttleasy.DAL.Models.dto.ShuttleSessions.dto;
using shuttleasy.DAL.Models.dto.User.dto;
using shuttleasy.DAL.Models.dto.GeoPoints.dto;
using shuttleasy.DAL.Models.dto.PickupArea.dto;
using shuttleasy.DAL.Models.dto.PickupPoint.dto;

namespace shuttleasy.Models.dto
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {          
            CreateMap<PassengerRegisterDto, Passenger>();
            CreateMap<CompanyWorkerRegisterDto, CompanyWorker>();
            CreateMap<PassengerRegisterPanelDto, Passenger>();
            CreateMap<UserProfileDto, Passenger>();
            CreateMap<DriverProfileDto, CompanyWorker>();
            CreateMap<DestinationDto, Destination>();
            CreateMap<ShuttleBusDto, ShuttleBus>();
            CreateMap<ShuttleSessionDto, ShuttleSession>();
            CreateMap<GeoPointDto, GeoPoint>();
            CreateMap<PickupAreaDto, PickupArea>();
            CreateMap<PickupPointDto, PickupPoint>();

            CreateMap<CompanyWorker, CompanyWorkerInfoDto>();
            CreateMap<Passenger, PassengerInfoDto>();

            CreateMap<ShuttleSession, ShuttleSessionSearchDto>();
        }
    }
}
