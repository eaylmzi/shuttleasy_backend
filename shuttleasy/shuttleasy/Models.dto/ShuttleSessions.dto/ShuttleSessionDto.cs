namespace shuttleasy.Models.dto.ShuttleSessions.dto
{
    public class ShuttleSessionDto
    {
        public int CompanyId { get; set; }
        public int BusId { get; set; }
        public int PassengerCount { get; set; }
        public DateTime StartTime { get; set; }
        public string StartingLongtitude { get; set; } = null!;
        public string StartingLatitude { get; set; } = null!;
        public bool IsActive { get; set; }
        public int DestinationId { get; set; }
    }
}
