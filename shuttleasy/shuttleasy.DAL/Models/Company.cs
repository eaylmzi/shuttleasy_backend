using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class Company
    {
        public Company()
        {
            CompanyWorkers = new HashSet<CompanyWorker>();
            ShuttleBus = new HashSet<ShuttleBu>();
            ShuttleSessions = new HashSet<ShuttleSession>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public double? Rating { get; set; }

        public virtual ICollection<CompanyWorker> CompanyWorkers { get; set; }
        public virtual ICollection<ShuttleBu> ShuttleBus { get; set; }
        public virtual ICollection<ShuttleSession> ShuttleSessions { get; set; }
    }
}
