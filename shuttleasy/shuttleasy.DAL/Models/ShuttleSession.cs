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
        public string StartingLongtitude { get; set; } = null!;
        public string StartingLatitude { get; set; } = null!;
        public bool IsActive { get; set; }
        public int DestinationId { get; set; }
    }
}
