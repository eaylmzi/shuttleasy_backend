using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class ShuttleBu
    {
        public ShuttleBu()
        {
            ShuttleSessions = new HashSet<ShuttleSession>();
        }

        public int Id { get; set; }
        public int Capacity { get; set; }
        public string BusModel { get; set; } = null!;
        public int Year { get; set; }
        public int CompanyId { get; set; }
        public string LicensePlate { get; set; } = null!;
        public bool? State { get; set; }

        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<ShuttleSession> ShuttleSessions { get; set; }
    }
}
