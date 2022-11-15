using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class SessionHistory
    {
        public string DriverId { get; set; } = null!;
        public int SessionId { get; set; }
        public DateTime Date { get; set; }
        public double? Rate { get; set; }
        public int? RateCount { get; set; }

        public virtual CompanyWorker Driver { get; set; } = null!;
        public virtual ShuttleSession Session { get; set; } = null!;
    }
}
