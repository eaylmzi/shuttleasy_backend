namespace shuttleasy.DAL.Models.dto.ShuttleBuses.dto
{
    public class ShuttleBusDto
    {
        public int Capacity { get; set; }
        public string BusModel { get; set; } = null!;
        public int CompanyId { get; set; }
        public string LicensePlate { get; set; } = null!;
        public bool? State { get; set; }
    }
}
