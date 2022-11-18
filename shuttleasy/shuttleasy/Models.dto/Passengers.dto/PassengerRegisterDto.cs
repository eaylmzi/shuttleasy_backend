namespace shuttleasy.Models.dto.Passengers.dto
{
    public class PassengerRegisterDto
    {
        public string IdentityNum { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public string City { get; set; } = null!;
        public string PassengerAddress { get; set; } = null!;

    }
}
