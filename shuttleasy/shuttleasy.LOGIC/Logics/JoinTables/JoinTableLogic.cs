using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Models;
using shuttleasy.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shuttleasy.DAL.Models.dto.Passengers.dto;
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using shuttleasy.DAL.Models.dto.Session.dto;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.DAL.Models.dto.PassengerShuttles.dto;
using shuttleasy.DAL.Models.dto.SessionPassengers.dto;

namespace shuttleasy.LOGIC.Logics.JoinTables
{
    public class JoinTableLogic : IJoinTableLogic
    {
        ShuttleasyDBContext _context = new ShuttleasyDBContext();

        private DbSet<Company> CompanyTable { get; set; }
        private DbSet<ShuttleSession> ShuttleSessionTable { get; set; }
        private DbSet<PassengerRating> PassengerRatingTable { get; set; }    
        private DbSet<Passenger> PassengerTable { get; set; }
        private DbSet<SessionPassenger> SessionPassengerTable { get; set; }
        private DbSet<GeoPoint> GeoPointTable { get; set; }
        private DbSet<ShuttleBus> ShuttleBusTable { get; set; }
        private DbSet<PickupPoint> PickupPointTable { get; set; }
        private DbSet<CompanyWorker> CompanyWorkerTable { get; set; }
        private DbSet<PickupArea> PickupAreaTable { get; set; }
        private DbSet<DriversStatistic> DriverStaticticTable { get; set; }
        private DbSet<SessionHistory> SessionHistoryTable { get; set; }

        public JoinTableLogic()
        {
            CompanyTable = _context.Set<Company>();
            ShuttleSessionTable = _context.Set<ShuttleSession>();
            PassengerRatingTable = _context.Set<PassengerRating>();
            PassengerTable = _context.Set<Passenger>();
            SessionPassengerTable = _context.Set<SessionPassenger>();
            GeoPointTable = _context.Set<GeoPoint>(); 
            ShuttleBusTable = _context.Set<ShuttleBus>();
            PickupPointTable = _context.Set<PickupPoint>();
            CompanyWorkerTable = _context.Set<CompanyWorker>();
            PickupAreaTable = _context.Set<PickupArea>();
            DriverStaticticTable = _context.Set<DriversStatistic>();
            SessionHistoryTable = _context.Set<SessionHistory>();
        }


       
        public List<ShuttleDetailsGroupDto> ShuttleDetailsInnerJoinTables(string destinationName)

         {


            var result = (from t1 in CompanyTable
                          join t2 in ShuttleSessionTable on t1.Id equals t2.CompanyId
                          join t3 in PassengerRatingTable
                              on t2.Id equals t3.SessionId into passengerRatings
                          from pr in passengerRatings.DefaultIfEmpty()
                          join t4 in PassengerTable
                              on pr.PassengerIdentity equals t4.Id into passengers
                          from p in passengers.DefaultIfEmpty()

                          join t5 in ShuttleBusTable on t2.BusId equals t5.Id 
                          join t6 in GeoPointTable on t2.FinalGeopoint equals t6.Id
                          where t2.DestinationName.ToLower().Contains(destinationName.ToLower())
                          select new ShuttleDetailsDto
                          {
                              CompanyDetails = t1,
                              ShuttleSessionDeparture = t2.Return == false ? new ShuttleSessionDetailsDto
                              {
                                  Id = t2.Id,
                                  CompanyId = t2.CompanyId,
                                  BusId = t2.BusId,
                                  PassengerCount = t2.PassengerCount,
                                  StartTime = t2.StartTime,
                                  DriverId = t2.DriverId,
                                  IsActive = t2.IsActive,
                                  Longitude = t6.Longtitude,
                                  Latitude = t6.Latitude,
                                  DestinationName = t2.DestinationName,
                                  Return = t2.Return,
                                  SessionDate = t2.SessionDate,
                                  Capacity = t5.Capacity,
                                  BusModel = t5.BusModel,
                                  LicensePlate = t5.LicensePlate,
                                  State = t5.State

                              } : null,
                              ShuttleSessionReturn = t2.Return == true ? new ShuttleSessionDetailsDto
                              {
                                  Id = t2.Id,
                                  CompanyId = t2.CompanyId,
                                  BusId = t2.BusId,
                                  PassengerCount = t2.PassengerCount,
                                  StartTime = t2.StartTime,
                                  DriverId = t2.DriverId,
                                  IsActive = t2.IsActive,
                                  Longitude = t6.Longtitude,
                                  Latitude = t6.Latitude,
                                  DestinationName = t2.DestinationName,
                                  Return = t2.Return,
                                  SessionDate = t2.SessionDate,
                                  Capacity = t5.Capacity,
                                  BusModel = t5.BusModel,
                                  LicensePlate = t5.LicensePlate,
                                  State = t5.State

                              } : null,

                          }).ToList();

            var groupedResult = result.GroupBy(x => x.CompanyDetails.Id)
                                      .Select(g => new ShuttleDetailsGroupDto
                                      {
                                          CompanyDetail = g.First().CompanyDetails,
                                          ShuttleSessionDeparture = g.Where(x => x.ShuttleSessionDeparture != null)
                                                                      .Select(x => x.ShuttleSessionDeparture)
                                                                      .ToList(),
                                          ShuttleSessionReturn = g.Where(x => x.ShuttleSessionReturn != null)
                                                                   .Select(x => x.ShuttleSessionReturn)
                                                                   .ToList(),
                                       
                                      }).ToList();

            return groupedResult;

        }
        public List<ShuttleDetailsGroupDto> ShuttleDetailsInnerJoinTables(int driverId)

        {
            var result = (from t1 in CompanyTable
                          join t2 in ShuttleSessionTable on t1.Id equals t2.CompanyId
                          join t3 in PassengerRatingTable
                              on t2.Id equals t3.SessionId into passengerRatings
                          from pr in passengerRatings.DefaultIfEmpty()
                          join t4 in PassengerTable
                              on pr.PassengerIdentity equals t4.Id into passengers
                          from p in passengers.DefaultIfEmpty()

                          join t5 in ShuttleBusTable on t2.BusId equals t5.Id
                          join t6 in GeoPointTable on t2.FinalGeopoint equals t6.Id
                          where t2.DriverId == driverId
                          select new ShuttleDetailsDto
                          {
                              CompanyDetails = t1,
                              ShuttleSessionDeparture = t2.Return == false ? new ShuttleSessionDetailsDto
                              {
                                  Id = t2.Id,
                                  CompanyId = t2.CompanyId,
                                  BusId = t2.BusId,
                                  PassengerCount = t2.PassengerCount,
                                  StartTime = t2.StartTime,
                                  DriverId = t2.DriverId,
                                  IsActive = t2.IsActive,
                                  Longitude = t6.Longtitude,
                                  Latitude = t6.Latitude,
                                  DestinationName = t2.DestinationName,
                                  Return = t2.Return,
                                  SessionDate = t2.SessionDate,
                                  Capacity = t5.Capacity,
                                  BusModel = t5.BusModel,
                                  LicensePlate = t5.LicensePlate,
                                  State = t5.State

                              } : null,
                              ShuttleSessionReturn = t2.Return == true ? new ShuttleSessionDetailsDto
                              {
                                  Id = t2.Id,
                                  CompanyId = t2.CompanyId,
                                  BusId = t2.BusId,
                                  PassengerCount = t2.PassengerCount,
                                  StartTime = t2.StartTime,
                                  DriverId = t2.DriverId,
                                  IsActive = t2.IsActive,
                                  Longitude = t6.Longtitude,
                                  Latitude = t6.Latitude,
                                  DestinationName = t2.DestinationName,
                                  Return = t2.Return,
                                  SessionDate = t2.SessionDate,
                                  Capacity = t5.Capacity,
                                  BusModel = t5.BusModel,
                                  LicensePlate = t5.LicensePlate,
                                  State = t5.State

                              } : null,

                          }).ToList();

            var groupedResult = result.GroupBy(x => x.CompanyDetails.Id)
                                      .Select(g => new ShuttleDetailsGroupDto
                                      {
                                          CompanyDetail = g.First().CompanyDetails,
                                          ShuttleSessionDeparture = g.Where(x => x.ShuttleSessionDeparture != null)
                                                                      .Select(x => x.ShuttleSessionDeparture)
                                                                      .ToList(),
                                          ShuttleSessionReturn = g.Where(x => x.ShuttleSessionReturn != null)
                                                                   .Select(x => x.ShuttleSessionReturn)
                                                                   .ToList(),

                                      }).ToList();

            return groupedResult;

        }
        public List<ShuttleDetailsGroupDto> ShuttleDetailsByGeoPointInnerJoinTables(string longitude,string latitude)

        {


            var result = (from t1 in CompanyTable
                          join t2 in ShuttleSessionTable on t1.Id equals t2.CompanyId
                          join t3 in PassengerRatingTable
                              on t2.Id equals t3.SessionId into passengerRatings
                          from pr in passengerRatings.DefaultIfEmpty()
                          join t4 in PassengerTable
                              on pr.PassengerIdentity equals t4.Id into passengers
                          from p in passengers.DefaultIfEmpty()
                          join t5 in ShuttleBusTable on t2.BusId equals t5.Id
                          join t6 in GeoPointTable on t2.FinalGeopoint equals t6.Id
                          where t6.Longtitude == longitude && t6.Latitude ==latitude
                          select new ShuttleDetailsDto
                          {
                              CompanyDetails = t1,
                              ShuttleSessionDeparture = t2.Return == false ? new ShuttleSessionDetailsDto
                              {
                                  Id = t2.Id,
                                  CompanyId = t2.CompanyId,
                                  BusId = t2.BusId,
                                  PassengerCount = t2.PassengerCount,
                                  StartTime = t2.StartTime,
                                  DriverId = t2.DriverId,
                                  IsActive = t2.IsActive,
                                  Longitude = t6.Longtitude,
                                  Latitude = t6.Latitude,
                                  DestinationName = t2.DestinationName,
                                  Return = t2.Return,
                                  SessionDate = t2.SessionDate,
                                  Capacity = t5.Capacity,
                                  BusModel = t5.BusModel,
                                  LicensePlate = t5.LicensePlate,
                                  State = t5.State

                              } : null,
                              ShuttleSessionReturn = t2.Return == true ? new ShuttleSessionDetailsDto
                              {
                                  Id = t2.Id,
                                  CompanyId = t2.CompanyId,
                                  BusId = t2.BusId,
                                  PassengerCount = t2.PassengerCount,
                                  StartTime = t2.StartTime,
                                  DriverId = t2.DriverId,
                                  IsActive = t2.IsActive,
                                  Longitude = t6.Longtitude,
                                  Latitude = t6.Latitude,
                                  DestinationName = t2.DestinationName,
                                  Return = t2.Return,
                                  SessionDate = t2.SessionDate,
                                  Capacity = t5.Capacity,
                                  BusModel = t5.BusModel,
                                  LicensePlate = t5.LicensePlate,
                                  State = t5.State

                              } : null,

                          }).ToList();

            var groupedResult = result.GroupBy(x => x.CompanyDetails.Id)
                                      .Select(g => new ShuttleDetailsGroupDto
                                      {
                                          CompanyDetail = g.First().CompanyDetails,
                                          ShuttleSessionDeparture = g.Where(x => x.ShuttleSessionDeparture != null)
                                                                      .Select(x => x.ShuttleSessionDeparture)
                                                                      .ToList(),
                                          ShuttleSessionReturn = g.Where(x => x.ShuttleSessionReturn != null)
                                                                   .Select(x => x.ShuttleSessionReturn)
                                                                   .ToList(),

                                      }).ToList();

            return groupedResult;

        }
        public List<CommentDetailsDto> CommentDetailsInnerJoinTables(int companyId)

        {

            var result = (from t1 in ShuttleSessionTable
                          join t2 in PassengerRatingTable on t1.Id equals t2.SessionId
                          join t3 in PassengerTable on t2.PassengerIdentity equals t3.Id
                          where t1.CompanyId == companyId
                          select new CommentDetailsDto
                          {
                              PassengerIdentity = t2.PassengerIdentity,
                              Rating = t2.Rating,
                              SessionId = t2.SessionId,
                              Date = t2.Date,
                              Comment = t2.Comment,
                              CompanyId = t1.CompanyId,
                              Name = t3.Name,
                              Surname = t3.Surname,
                              ProfilePic = t3.ProfilePic
                             
                          }).ToList();

            return result;

        }
        public List<CompanyDetailGroupDto> CompanyDetailsInnerJoinTables(int companyId)

        {
            var result = (from t1 in ShuttleSessionTable
                          join t2 in PassengerRatingTable on t1.Id equals t2.SessionId into t2Group
                          from t2 in t2Group.DefaultIfEmpty()
                          join t3 in CompanyTable on t1.CompanyId equals t3.Id
                          join t4 in PassengerTable on t2.PassengerIdentity equals t4.Id into t4Group
                          from t4 in t4Group.DefaultIfEmpty()

                          where t3.Id == companyId
                          select new CompanyDetailDto
                          {
                              Company = t3,
                              CommentDetails = (t2 == null) ? null : new CommentDetailsDto
                              {
                                  PassengerIdentity = t4.Id,
                                  Rating = t2.Rating,
                                  SessionId = t2.SessionId,
                                  Date = t2.Date,
                                  Comment = t2.Comment,
                                  CompanyId = t3.Id,
                                  Name = t4.Name,
                                  Surname = t4.Surname,
                                  ProfilePic = t4.ProfilePic
                              }
                          }).ToList();

            var groupedResult = result.GroupBy(x => x.Company.Id)
                                      .Select(g => {
                                          var commentDetails = g.Where(x => x.CommentDetails != null)
                                                               .Select(x => x.CommentDetails)
                                                               .ToList();

                                          if (commentDetails.Count > 0)
                                          {
                                              return new CompanyDetailGroupDto
                                              {
                                                  Company = g.First().Company,
                                                  CommentDetails = commentDetails
                                              };
                                          }
                                          else
                                          {
                                              return new CompanyDetailGroupDto
                                              {
                                                  Company = g.First().Company,
                                                  CommentDetails = null
                                              };
                                          }
                                      }).ToList();

            return groupedResult;
            /*
            var result = (from t1 in ShuttleSessionTable
                          join t2 in PassengerRatingTable on t1.Id equals t2.SessionId into t2Group
                          from t2 in t2Group.DefaultIfEmpty()
                          join t3 in CompanyTable on t1.CompanyId equals t3.Id
                          join t4 in PassengerTable on t2.PassengerIdentity equals t4.Id into t4Group
                          from t4 in t4Group.DefaultIfEmpty()
                          
                          where t3.Id == companyId
                          select new CompanyDetailDto
                          {
                              Company = t3,
                              CommentDetails = (t2 == null) ? null : new CommentDetailsDto
                              {
                                  PassengerIdentity = t4.Id,
                                  Rating = t2.Rating,
                                  SessionId = t2.SessionId,
                                  Date = t2.Date,
                                  Comment = t2.Comment,
                                  CompanyId = t3.Id,
                                  Name = t4.Name,
                                  Surname = t4.Surname,
                                  ProfilePic = t4.ProfilePic
                              }
                          }).ToList();
            var groupedResult = result.GroupBy(x => x.Company.Id)
                                    .Select(g => new CompanyDetailGroupDto
                                    {
                                        Company = g.First().Company,
                                        CommentDetails = g.Where(x => x.CommentDetails != null)
                                                                    .Select(x => x.CommentDetails)
                                                                    .ToList(),

                                    }).ToList();

            return groupedResult;
            */

        }
        public List<PassengerShuttleDetailsDto> PassengerShuttleInnerJoinTables(int userId)

        {

            var result = (from t1 in SessionPassengerTable
                          join t2 in PickupPointTable on t1.PickupId equals t2.Id
                          where t2.UserId == userId

                          select new PassengerShuttleDetailsDto
                          {
                              Id =t1.Id,
                              SessionId = t1.SessionId,
                              EstimatedPickupTime = t1.EstimatedPickupTime,
                              PickupOrderNum = t1.PickupOrderNum,
                              PickupState = t1.PickupState,
                              PickupId = t1.PickupId,
                              UserId = t2.UserId,
                              GeoPointId = t2.GeoPointId,
                          }).ToList();
            return result;
           

        }       
        public List<EnrolledPassengersGroupDto> ShuttlePassengersInnerJoinTables(int companyId)

        {

            var result = (from t1 in CompanyTable
                          join t2 in ShuttleSessionTable on t1.Id equals t2.CompanyId
                          join t3 in SessionPassengerTable on t2.Id equals t3.SessionId
                          join t4 in PickupPointTable on t3.PickupId equals t4.Id
                          join t5 in PassengerTable on t4.UserId equals t5.Id
                          where t1.Id == companyId

                          select new EnrolledPassengersDto
                          {
                              ShuttleSession = t2,
                              PassengerDetailsDto = new PassengerDetailsDto
                              {
                                  ProfilePic = t5.ProfilePic,
                                  Name = t5.Name,
                                  Surname =t5.Surname,
                                  PhoneNumber = t5.PhoneNumber,
                                  Email = t5.Email,
                                  City = t5.City,
                                  PassengerAddress = t5.PassengerAddress,
                              },
                              
                          }).ToList();
            var groupedResult = result.GroupBy(x => x.ShuttleSession.Id)
                                  .Select(g => new EnrolledPassengersGroupDto
                                  {
                                      ShuttleSession = g.First().ShuttleSession,
                                      PassengerDetailsDtoList = g.Where(x => x.PassengerDetailsDto != null)
                                                                  .Select(x => x.PassengerDetailsDto)
                                                                  .ToList(),
                                  }).ToList();

            return groupedResult;


        }
        public List<ShuttleSession> DriverShuttleInnerJoinTables(int driverId)

        {

            var result = (from t1 in CompanyWorkerTable
                          join t2 in ShuttleSessionTable on t1.Id equals t2.DriverId
                          where t1.Id == driverId

                          select new ShuttleSession
                          {
                              Id = t2.Id,
                              CompanyId = t2.CompanyId,
                              BusId = t2.BusId,
                              PassengerCount = t2.PassengerCount,
                              StartTime = t2.StartTime,
                              DriverId = t2.DriverId,
                              IsActive = t2.IsActive,
                              StartGeopoint = t2.StartGeopoint,
                              FinalGeopoint = t2.FinalGeopoint,
                              DestinationName = t2.DestinationName,
                              Return = t2.Return,
                              SessionDate = t2.SessionDate,


                          }).ToList();


            return result;


        }
        public List<PassengerDetailsDto> PassengerSessionPassengerJoinTables(int sessionId)

        {

            var result = (from t1 in PassengerTable
                          join t2 in PickupPointTable on t1.Id equals t2.UserId
                          join t3 in SessionPassengerTable on t2.Id equals t3.PickupId
                          where t3.SessionId == sessionId

                          select new PassengerDetailsDto
                          {
                              ProfilePic = t1.ProfilePic,
                              Name = t1.Name,
                              Surname = t1.Surname,
                              PhoneNumber = t1.PhoneNumber,
                              Email = t1.Email,
                              City = t1.City,
                              PassengerAddress = t1.PassengerAddress,
                          }).ToList();




            return result;


        }
        public List<SessionGeoPointsDto> SessionGeoPointsInnerJoinTables(int? sessionId)

        {

            if (sessionId == null)
            {
                throw new ArgumentNullException(nameof(sessionId), "Session ID cannot be null.");
            }

            var result = (from t1 in SessionPassengerTable
                          join t2 in GeoPointTable on t1.PickupId equals t2.Id
                          where t1.SessionId == sessionId
                          select new SessionGeoPointsDto
                          {
                              Longtitude = t2.Longtitude,
                              Latitude = t2.Latitude,
                          }).ToList();

            return result;

        }
        public List<PickupArea> ShuttlePickUpAreaInnerJoinTables(List<int> sessionId)

        {
            var result = (from t1 in ShuttleSessionTable
                          join t2 in PickupAreaTable on t1.Id equals t2.SessionId
                          where sessionId.Contains(t1.Id)
                          select new PickupArea
                          {
                              Id = t2.Id,
                              SessionId = t2.SessionId,
                              PolygonPoints = t2.PolygonPoints,
                          }).ToList();

            return result;
        }
        public List<SessionPassengerPickupIdDetailsDto> SessionPassengerPickupPointJoinTables(int sessionId)

        {
            var result = (from t1 in SessionPassengerTable
                          join t2 in PickupPointTable on t1.PickupId equals t2.Id
                          join t3 in GeoPointTable on t2.GeoPointId equals t3.Id
                          join t4 in PassengerTable on t2.UserId equals t4.Id
                          where t1.SessionId == sessionId

                          select new SessionPassengerPickupIdDetailsDto
                          {
                              UserId = t2.UserId,
                              NotificationToken = t4.NotificationToken,
                              Latitude = t3.Latitude,
                              Longtitude = t3.Longtitude

                          }).ToList();
            return result;
        }

        public List<StartFinishTime> ShuttleSessionDriverStaticticJoinTables(int sessionId)

        {
            var result = (from t1 in ShuttleSessionTable
                          join t2 in SessionHistoryTable on t1.Id equals t2.SessionId
                          where t1.Id == sessionId

                          select new StartFinishTime
                          {
                              StartTime = t1.StartTime,
                              FinalTime = t2.Date,

                          }).ToList();
            return result;
        }
        public List<PassengerRouteDto> ShuttleManagerJoinTables(int sessionId)

        {
            var result = (from t1 in SessionPassengerTable
                          join t2 in PickupPointTable on t1.PickupId equals t2.Id
                          join t3 in GeoPointTable on t2.GeoPointId equals t3.Id
                          join t4 in PassengerTable on t2.UserId equals t4.Id
                          where t1.SessionId == sessionId

                          select new PassengerRouteDto
                          {
                              UserId = t2.UserId,
                              NotificationToken = t4.NotificationToken,
                              Latitude = t3.Latitude,
                              Longtitude = t3.Longtitude,
                              EstimatedArriveTime = null,

                          }).ToList();
            return result;
        }
        public List<DriversInfoDto> CompanyWorkerDriverStaticticRatingAvgJoinTables(int companyId)

        {
            var result = (from t1 in CompanyWorkerTable
                          join t2 in DriverStaticticTable on t1.Id equals t2.DriverId
                          where t1.CompanyId == companyId
                          orderby t2.RatingAvg descending

                          select new DriversInfoDto
                          {
                              Name = t1.Name,
                              Surname = t1.Surname,
                              PhoneNumber = t1.PhoneNumber,
                              RateCount = t2.RateCount,
                              RatingAvg = t2.RatingAvg,
                              WorkingHours = t2.WorkingHours,

                          }).ToList();
            return result;
        }
        public List<DriversInfoDto> CompanyWorkerDriverStaticticByNameJoinTables(int companyId)
        {
            var result = (from t1 in CompanyWorkerTable
                          join t2 in DriverStaticticTable on t1.Id equals t2.DriverId
                          where t1.CompanyId == companyId
                          orderby t1.Name
                          select new DriversInfoDto
                          {
                              Name = t1.Name,
                              Surname = t1.Surname,
                              PhoneNumber = t1.PhoneNumber,
                              RateCount = t2.RateCount,
                              RatingAvg = t2.RatingAvg,
                              WorkingHours = t2.WorkingHours,
                          }).ToList();
            return result;
        }
        public List<DriversInfoDto> CompanyWorkerDriverStaticticWorkingHoursJoinTables(int companyId)

        {
            var result = (from t1 in CompanyWorkerTable
                          join t2 in DriverStaticticTable on t1.Id equals t2.DriverId
                          where t1.CompanyId == companyId
                          orderby t2.WorkingHours descending

                          select new DriversInfoDto
                          {
                              Name = t1.Name,
                              Surname = t1.Surname,
                              PhoneNumber = t1.PhoneNumber,
                              RateCount = t2.RateCount,
                              RatingAvg = t2.RatingAvg,
                              WorkingHours = t2.WorkingHours,

                          }).ToList();
            return result;
        }
        public List<SessionPassenger> GetSessionPassengerJoinTables(int userId, int sessionId)

        {
            var result = (from t1 in SessionPassengerTable
                          join t2 in PickupPointTable on t1.PickupId equals t2.Id
                          where t1.SessionId == sessionId && t2.UserId == userId


                          select new SessionPassenger
                          {
                              Id = t1.Id,
                              SessionId = t1.SessionId,
                              EstimatedPickupTime = t1.EstimatedPickupTime,
                              PickupOrderNum = t1.PickupOrderNum,
                              PickupState = t1.PickupState,
                              PickupId = t1.PickupId,

                          }).ToList();
            return result;
        }
        public List<SessionPassengerAndPassengerId> GetSessionPassengerAndPassengerIdJoinTables(int sessionId)

        {
            var result = (from t1 in SessionPassengerTable
                          join t2 in PickupPointTable on t1.PickupId equals t2.Id
                          where t1.SessionId == sessionId


                          select new SessionPassengerAndPassengerId
                          {
                              Id = t1.Id,
                              UserId = t2.UserId,
                              SessionId = t1.SessionId,
                              EstimatedPickupTime = t1.EstimatedPickupTime,
                              PickupOrderNum = t1.PickupOrderNum,
                              PickupState = t1.PickupState,
                              PickupId = t1.PickupId,

                          }).ToList();
            return result;
        }
        public List<PassengerRouteDto> PassengerRouteJoinTables(int sessionId)

        {

            var result = (from t1 in SessionPassengerTable
                          join t2 in PickupPointTable on t1.PickupId equals t2.Id
                          join t3 in PassengerTable on t2.UserId equals t3.Id
                          join t4 in GeoPointTable on t2.GeoPointId equals t4.Id
                          where t1.SessionId == sessionId 
                          orderby t1.PickupOrderNum ascending


                          select new PassengerRouteDto
                          {
                              UserId = t3.Id,
                              Latitude = t4.Latitude,
                              Longtitude = t4.Longtitude,
                              EstimatedArriveTime = t1.EstimatedPickupTime,
                              NotificationToken = t3.NotificationToken,



                          }).ToList();
            return result;
        }
        public List<PassengerShuttleDto>? GetRouteJoinTables(int sessionId)

        {

            var result = (from t1 in ShuttleSessionTable
                          join t2 in CompanyTable on t1.CompanyId equals t2.Id
                          join t3 in CompanyWorkerTable on t1.DriverId equals t3.Id
                          where t1.Id == sessionId


                          select new PassengerShuttleDto
                          {
                             PassengerCount = t1.PassengerCount,
                             StartTime = t1.StartTime,
                             ShuttleState = t1.ShuttleState,
                             DriverName = t3.Name,
                             CompanyName = t2.Name,
                             RoutePoints = null,



                          }).ToList();
            return result;
        }
        public List<ShuttleDto> MertimYapmaz(int driverId)
        {
            var result = (from t1 in CompanyTable
                          join t2 in ShuttleSessionTable on t1.Id equals t2.CompanyId
                          join t3 in PassengerRatingTable
                              on t2.Id equals t3.SessionId into passengerRatings
                          from pr in passengerRatings.DefaultIfEmpty()
                          join t4 in PassengerTable
                              on pr.PassengerIdentity equals t4.Id into passengers
                          from p in passengers.DefaultIfEmpty()

                          join t5 in ShuttleBusTable on t2.BusId equals t5.Id
                          join t6 in GeoPointTable on t2.FinalGeopoint equals t6.Id
                          join t7 in GeoPointTable on t2.StartGeopoint equals t7.Id
                          where t2.DriverId == driverId
                          select new ShuttleDto
                          {
        
                                  Id = t2.Id,
                                  CompanyId = t2.CompanyId,
                                  BusId = t2.BusId,
                                  PassengerCount = t2.PassengerCount,
                                  StartTime = t2.StartTime,
                                  DriverId = t2.DriverId,
                                  IsActive = t2.IsActive,
                                  LongitudeStart = t7.Longtitude,
                                  LatitudeStart = t7.Latitude,
                                  StartName = t7.LocationName,
                                  LongitudeFinal = t6.Longtitude,
                                  LatitudeFinal = t6.Latitude,
                                  DestinationName = t2.DestinationName,
                                  Return = t2.Return,
                                  SessionDate = t2.SessionDate,
                                  Capacity = t5.Capacity,
                                  BusModel = t5.BusModel,
                                  LicensePlate = t5.LicensePlate,
                                  State = t5.State


                          }).ToList();
            return result;
        }
        public List<ShuttleDto> OzimYapmaz(int passengerId)
        {
            var result = (from t1 in PickupPointTable
                          join t2 in SessionPassengerTable on t1.Id equals t2.PickupId
                          join t3 in ShuttleSessionTable on t2.SessionId equals t3.Id
                          join t4 in CompanyTable on t3.CompanyId equals t4.Id
                          join t5 in GeoPointTable on t3.StartGeopoint equals t5.Id
                          join t6 in GeoPointTable on t3.FinalGeopoint equals t6.Id
                          join t7 in ShuttleBusTable on t3.BusId equals t7.Id

                          where t1.UserId == passengerId
                          select new ShuttleDto
                          {

                              Id = t2.Id,
                              CompanyId = t4.Id,
                              BusId = t3.BusId,
                              PassengerCount = t3.PassengerCount,
                              StartTime = t3.StartTime,
                              DriverId = t3.DriverId,
                              IsActive = t3.IsActive,
                              LongitudeStart = t5.Longtitude,
                              LatitudeStart = t5.Latitude,
                              StartName = t5.LocationName,
                              LongitudeFinal = t6.Longtitude,
                              LatitudeFinal = t6.Latitude,
                              DestinationName = t3.DestinationName,
                              Return = t3.Return,
                              SessionDate = t3.SessionDate,
                              Capacity = t7.Capacity,
                              BusModel = t7.BusModel,
                              LicensePlate = t7.LicensePlate,
                              State = t7.State


                          }).ToList();
            return result;
        }
        //YUKARDIKİNİN FOREACHLİ ÖRNEĞİ
        /*   public List<PickupArea> ShuttlePickUpAresaInnerJoinTables(List<int> sessionIds)

           {

               var pickupAreas = new List<PickupArea>();

               foreach (int sessionId in sessionIds)
               {
                   var result = (from t1 in ShuttleSessionTable
                                 join t2 in PickupAreaTable on t1.Id equals t2.SessionId
                                 where t1.Id == sessionId
                                 select new PickupArea
                                 {
                                     Id = t2.Id,
                                     SessionId = t2.SessionId,
                                     PolygonPoints = t2.PolygonPoints,
                                 }).ToList();

                   pickupAreas.AddRange(result);
               }

               return pickupAreas;

           }
        */


    }
}
