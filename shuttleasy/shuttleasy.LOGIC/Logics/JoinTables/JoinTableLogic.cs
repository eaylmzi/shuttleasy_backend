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

namespace shuttleasy.LOGIC.Logics.JoinTables
{
    public class JoinTableLogic : IJoinTableLogic
    {
        ShuttleasyDBContext _context = new ShuttleasyDBContext();

        private DbSet<Company> CompanyTable { get; set; }
        private DbSet<ShuttleSession> ShuttleSessionTable { get; set; }
        private DbSet<PassengerRating> PassengerRatingTable { get; set; }
        private DbSet<SessionPassenger> SessionPassengerTable { get; set; }
        private DbSet<Passenger> PassengerTable { get; set; }
        private DbSet<GeoPoint> GeoPointTable { get; set; }



       
    
        
        public JoinTableLogic()
        {
            CompanyTable = _context.Set<Company>();
            ShuttleSessionTable = _context.Set<ShuttleSession>();
            PassengerRatingTable = _context.Set<PassengerRating>();
            PassengerTable = _context.Set<Passenger>();
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
                          where t2.DestinationName == destinationName
                          select new ShuttleDetailsDto
                          {
                              CompanyDetails = t1,
                              ShuttleSessionDeparture = t2.Return == false ? t2 : null,
                              ShuttleSessionReturn = t2.Return == true ? t2 : null,
                             
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
                          join t2 in PassengerRatingTable on t1.Id equals t2.SessionId
                          join t3 in CompanyTable on t1.CompanyId equals t3.Id
                          join t4 in PassengerTable on t2.PassengerIdentity equals t4.Id
                          where t3.Id == companyId

                          select new CompanyDetailDto
                          {
                              Company = t3,
                              CommentDetails = new CommentDetailsDto
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



    }
}
