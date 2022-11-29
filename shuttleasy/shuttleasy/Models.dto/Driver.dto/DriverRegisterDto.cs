namespace shuttleasy.Models.dto.Driver.dto
{
    public class DriverRegisterDto
    {
        public string IdentityNum { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int CompanyId { get; set; }
        public string Email { get; set; } = null!;
        public string City { get; set; } = null!;
        public string PassengerAddress { get; set; } = null!;
        public string WorkerType { get; set; } = null!;

    }
}
