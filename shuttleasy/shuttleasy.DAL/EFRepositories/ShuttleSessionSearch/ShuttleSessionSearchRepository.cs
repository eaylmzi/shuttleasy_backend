using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.ShuttleSessionSearch
{
    public class ShuttleSessionSearchRepository
    {
        ShuttleasyDBContext _context = new ShuttleasyDBContext();
        
        private DbSet<ShuttleSession> table1 { get; set; }
        private DbSet<Destination> table2 { get; set; }

      
        public ShuttleSessionSearchRepository()
        {
            table1 = _context.Set<ShuttleSession>();
            table2 = _context.Set<Destination>();
        }

       
         public List<ShuttleSessionSearchDto> InnerJoinTables(string lastPoint)

         {

             var result =( from t1 in table1
                          join t2 in table2
                          on t1.DestinationId equals t2.Id
                           where t2.LastDestination == lastPoint
                           select new ShuttleSessionSearchDto
                           {
                               Id = t1.Id,
                               CompanyId = t1.CompanyId,
                               BusId = t1.BusId,
                               PassengerCount = t1.PassengerCount,
                               StartTime = t1.StartTime,
                               DriverId = t1.DriverId,
                               IsActive = t1.IsActive,
                               DestinationId = t1.DestinationId

                           }).ToList();






            return result;
         } 


 

    }
}
