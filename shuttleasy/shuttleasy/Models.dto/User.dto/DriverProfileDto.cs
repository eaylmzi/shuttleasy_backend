namespace shuttleasy.Models.dto.User.dto
{
    public class DriverProfileDto
    {
        public byte[]? ProfilePic { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;

    }
}
