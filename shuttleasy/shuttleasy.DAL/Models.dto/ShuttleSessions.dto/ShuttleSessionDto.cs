namespace shuttleasy.DAL.Models.dto.ShuttleSessions.dto
{
    public class ShuttleSessionDto
    {
        public int CompanyId { get; set; }
        public int BusId { get; set; }
        public int PassengerCount { get; set; }
        public DateTime StartTime { get; set; }
        public int DriverId { get; set; }
        public bool IsActive { get; set; }
        public int? StartGeopoint { get; set; }
        public int? FinalGeopoint { get; set; }
        public string DestinationName { get; set; } = null!;
        public bool Return { get; set; }
        public string SessionDate { get; set; } = null!;
        public double Price { get; set; } 
    }
}
