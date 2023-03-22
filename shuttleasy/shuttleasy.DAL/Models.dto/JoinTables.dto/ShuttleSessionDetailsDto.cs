using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.JoinTables.dto
{
    public class ShuttleSessionDetailsDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int BusId { get; set; }
        public int PassengerCount { get; set; }
        public DateTime StartTime { get; set; }
        public int DriverId { get; set; }
        public bool IsActive { get; set; }
        public string Longitude { get; set; } = null!;
        public string Latitude { get; set; } = null!;
        public string? DestinationName { get; set; }
        public bool Return { get; set; }
        public string SessionDate { get; set; } = null!;
        public int Capacity { get; set; }
        public string BusModel { get; set; } = null!;
        public string LicensePlate { get; set; } = null!;
        public bool? State { get; set; }
    }
}
