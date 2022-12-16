using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class ShuttleSession
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int BusId { get; set; }
        public int PassengerCount { get; set; }
        public DateTime StartTime { get; set; } 
        public int DriverId { get; set; }
        public bool IsActive { get; set; }
        public int DestinationId { get; set; }
    }
}
