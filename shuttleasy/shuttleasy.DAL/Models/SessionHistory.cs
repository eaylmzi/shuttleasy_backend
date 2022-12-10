using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class SessionHistory
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public int SessionId { get; set; }
        public DateTime Date { get; set; }
        public double? Rate { get; set; }
        public int? RateCount { get; set; }
    }
}
