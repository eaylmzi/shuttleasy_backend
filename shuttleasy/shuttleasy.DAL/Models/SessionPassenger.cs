using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class SessionPassenger
    {
        public int SessionId { get; set; }
        public DateTime SessionDate { get; set; }
        public string PassengerIdentity { get; set; } = null!;
        public byte[] EstimatedPickupTime { get; set; } = null!;
        public int PickupOrderNum { get; set; }
        public string PickupState { get; set; } = null!;
        public string? PickupLatitude { get; set; }
        public string? PickupLongtitude { get; set; }

        public virtual Passenger PassengerIdentityNavigation { get; set; } = null!;
        public virtual ShuttleSession Session { get; set; } = null!;
    }
}
