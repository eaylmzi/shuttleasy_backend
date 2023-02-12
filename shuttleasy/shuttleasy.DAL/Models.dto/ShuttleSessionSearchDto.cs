using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto
{
    public class ShuttleSessionSearchDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public int BusId { get; set; }
        public string BusLicensePlate { get; set; } = null!;
        public int PassengerCount { get; set; }
        public DateTime StartTime { get; set; }
        public int DriverId { get; set; }
        public bool IsActive { get; set; }
        public int DestinationId { get; set; }
    }
}
