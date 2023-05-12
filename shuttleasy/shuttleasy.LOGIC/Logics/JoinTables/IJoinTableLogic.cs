
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using shuttleasy.DAL.Models.dto.PassengerShuttles.dto;
using shuttleasy.DAL.Models.dto.Session.dto;
using shuttleasy.DAL.Models.dto.SessionPassengers.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static shuttleasy.LOGIC.Logics.JoinTables.JoinTableLogic;

namespace shuttleasy.LOGIC.Logics.JoinTables
{
    public interface IJoinTableLogic
    {
        public List<ShuttleDetailsGroupDto> ShuttleDetailsInnerJoinTables(string destinationName);
        public List<ShuttleDetailsGroupDto> ShuttleDetailsInnerJoinTables(int driverId);
        public List<ShuttleDetailsGroupDto> ShuttleDetailsByGeoPointInnerJoinTables(string longitude, string latitude);
        public List<CommentDetailsDto> CommentDetailsInnerJoinTables(int companyId);
        public List<CompanyDetailGroupDto> CompanyDetailsInnerJoinTables(int companyId);
        public List<SessionGeoPointsDto> SessionGeoPointsInnerJoinTables(int? sessionId);
        public List<PassengerShuttleDetailsDto> PassengerShuttleInnerJoinTables(int userId);
        public List<EnrolledPassengersGroupDto> ShuttlePassengersInnerJoinTables(int companyId);
        public List<ShuttleSession> DriverShuttleInnerJoinTables(int driverId);
        public List<PassengerDetailsDto> PassengerSessionPassengerJoinTables(int sessionId);
        public List<PickupArea> ShuttlePickUpAreaInnerJoinTables(List<int> sessionId);
        public List<SessionPassengerPickupIdDetailsDto> SessionPassengerPickupPointJoinTables(int sessionId);
        public List<StartFinishTime> ShuttleSessionDriverStaticticJoinTables(int sessionId);
        public List<PassengerRouteDto> ShuttleManagerJoinTables(int sessionId);
        public List<DriversInfoDto> CompanyWorkerDriverStaticticRatingAvgJoinTables(int companyId);
        public List<DriversInfoDto> CompanyWorkerDriverStaticticWorkingHoursJoinTables(int companyId);
        public List<DriversInfoDto> CompanyWorkerDriverStaticticByNameJoinTables(int companyId);
        public List<SessionPassenger> GetSessionPassengerJoinTables(int userId, int sessionId);
        public List<SessionPassengerAndPassengerId> GetSessionPassengerAndPassengerIdJoinTables(int sessionId);
        public List<PassengerRouteDto> PassengerRouteJoinTables(int sessionId);
        public List<PassengerShuttleDto>? GetRouteJoinTables(int sessionId);
        public List<ShuttleDto> MertimYapmaz(int driverId);
        public List<ShuttleDto> OzimYapmaz(int passengerId);
    }
}
