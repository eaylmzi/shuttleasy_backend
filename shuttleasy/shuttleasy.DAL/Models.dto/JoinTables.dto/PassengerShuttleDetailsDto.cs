using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.JoinTables.dto
{
    public class PassengerShuttleDetailsDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public DateTime? EstimatedPickupTime { get; set; }
        public int? PickupOrderNum { get; set; }
        public string? PickupState { get; set; }
        public int? PickupId { get; set; }
        public int UserId { get; set; }
        public int GeoPointId { get; set; }
    }
}
