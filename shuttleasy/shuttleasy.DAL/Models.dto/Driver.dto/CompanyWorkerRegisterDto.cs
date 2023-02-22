namespace shuttleasy.DAL.Models.dto.Driver.dto
{
    public class CompanyWorkerRegisterDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int CompanyId { get; set; }
        public string Email { get; set; } = null!;

    }
}
