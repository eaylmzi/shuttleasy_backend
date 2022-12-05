using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class SessionPassenger
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public DateTime SessionDate { get; set; }
        public int PassengerIdentity { get; set; }
        public byte[] EstimatedPickupTime { get; set; } = null!;
        public int PickupOrderNum { get; set; }
        public string PickupState { get; set; } = null!;
        public string? PickupLatitude { get; set; }
        public string? PickupLongtitude { get; set; }
    }
}
