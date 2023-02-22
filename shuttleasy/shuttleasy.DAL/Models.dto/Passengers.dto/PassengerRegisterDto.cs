namespace shuttleasy.DAL.Models.dto.Passengers.dto
{
    public class PassengerRegisterDto
    {      
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? PhoneNumber { get; set; } 
        public string Email { get; set; } = null!;
        public string? Password { get; set; }

    }
}
