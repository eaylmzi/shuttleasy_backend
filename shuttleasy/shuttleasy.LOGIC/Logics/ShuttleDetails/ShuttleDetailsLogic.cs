using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Models;
using shuttleasy.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shuttleasy.DAL.Models.dto.ShuttleDetails.dto;
using shuttleasy.DAL.Models.dto.Passengers.dto;

namespace shuttleasy.LOGIC.Logics.ShuttleDetails
{
    public class ShuttleDetailsLogic : IShuttleDetailsLogic
    {
        ShuttleasyDBContext _context = new ShuttleasyDBContext();

        private DbSet<Company> CompanyTable { get; set; }
        private DbSet<ShuttleSession> ShuttleSessionTable { get; set; }
        private DbSet<PassengerRating> PassengerRatingTable { get; set; }
        private DbSet<Passenger> PassengerTable { get; set; }


        public ShuttleDetailsLogic()
        {
            CompanyTable = _context.Set<Company>();
            ShuttleSessionTable = _context.Set<ShuttleSession>();
            PassengerRatingTable = _context.Set<PassengerRating>();
            PassengerTable = _context.Set<Passenger>();
        }

       
         public List<ShuttleDetailsDto> InnerJoinTables(int companyId)

         {

            var result = (from t1 in CompanyTable
                          join t2 in ShuttleSessionTable on t1.Id equals t2.CompanyId
                          join t3 in PassengerRatingTable
                              on t2.Id equals t3.SessionId into passengerRatings
                          from pr in passengerRatings.DefaultIfEmpty()
                          join t4 in PassengerTable
                              on pr.PassengerIdentity equals t4.Id into passengers
                          from p in passengers.DefaultIfEmpty()
                          where t1.Id == companyId
                          select new ShuttleDetailsDto
                          {
                              CompanyDetails = t1,
                              ShuttleSessionDeparture = t2.Return == false ? t2 : null,
                              ShuttleSessionReturn = t2.Return == true ? t2 : null,
                              PassengerComment = p != null && pr != null ? new PassengerCommentDto
                              {
                                  Name = p.Name,
                                  Surname = p.Surname,
                                  Comments = pr.Comment
                              } : null
                          }).ToList();

            return result;

         } 
    }
}
