namespace shuttleasy.Models.dto.Destinations.dto
{
    public class DestinationDto
    {
        public int CityNumber { get; set; }
        public string BeginningDestination { get; set; } = null!;
        public string LastDestination { get; set; } = null!;
    }
}
