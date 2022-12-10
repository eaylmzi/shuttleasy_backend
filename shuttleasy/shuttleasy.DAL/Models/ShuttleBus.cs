using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class ShuttleBus
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public string BusModel { get; set; } = null!;
        public int CompanyId { get; set; }
        public string LicensePlate { get; set; } = null!;
        public bool? State { get; set; }
        public int DestinationId { get; set; }
    }
}
