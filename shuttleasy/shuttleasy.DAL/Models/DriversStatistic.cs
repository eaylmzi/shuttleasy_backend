using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class DriversStatistic
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public double RatingAvg { get; set; }
        public int RateCount { get; set; }
        public double WorkingHours { get; set; }
        public int SessionId { get; set; }

        public virtual CompanyWorker Driver { get; set; } = null!;
        public virtual ShuttleSession Session { get; set; } = null!;
    }
}
