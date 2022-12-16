using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class CompanyWorker
    {
        public int Id { get; set; }
        public byte[]? ProfilePic { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; } = null!;
        public bool Verified { get; set; }
        public string? Token { get; set; }
        public string WorkerType { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
    }
}
