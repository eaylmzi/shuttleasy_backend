using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.PassengerShuttles.dto
{
    public class PassengerShuttleDto
    {
        
        public int PassengerCount { get; set; } 
        public DateTime StartTime { get; set; } 
        public string ShuttleState { get; set; }
        public string CompanyName { get; set; } = null!;
        public string DriverName { get; set; }
        public List<double[]>? RoutePoints { get; set; }   
    }
}
