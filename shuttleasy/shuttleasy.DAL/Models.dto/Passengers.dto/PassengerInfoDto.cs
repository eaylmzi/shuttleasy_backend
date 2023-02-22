namespace shuttleasy.DAL.Models.dto.Passengers.dto
{
    public class PassengerInfoDto
    {
        public int Id { get; set; }
        public byte[]? ProfilePic { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? PassengerAddress { get; set; }
        public Guid QrString { get; set; }
        public string? Token { get; set; }
    }
}
