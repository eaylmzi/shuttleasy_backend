
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.JoinTables.dto;
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
    }
}
