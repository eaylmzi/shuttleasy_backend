namespace shuttleasy.DAL.Models.dto.Driver.dto
{
    public class CompanyWorkerInfoDto
    {
        public int Id { get; set; }
        public byte[]? ProfilePic { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string WorkerType { get; set; } = null!;

    }
}
