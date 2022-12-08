namespace shuttleasy.Models.dto.User.dto
{
    public class UserProfileDto
    {

        public byte[]? ProfilePic { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? PassengerAddress { get; set; }

    }
}
