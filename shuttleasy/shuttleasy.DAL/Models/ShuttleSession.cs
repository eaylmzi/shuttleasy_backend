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
        public int? StartGeopoint { get; set; }
        public int? FinalGeopoint { get; set; } 
        public string? DestinationName { get; set; } 
        public bool Return { get; set; }
        public string SessionDate { get; set; } = null!;
        public string? ShuttleState { get; set; } 

    }
}
