using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class SessionPassenger
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public DateTime? EstimatedPickupTime { get; set; } 
        public int? PickupOrderNum { get; set; }
        public string? PickupState { get; set; }
        public int? PickupId { get; set; }
    }
}
