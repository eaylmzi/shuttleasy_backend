using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.JoinTables.dto
{
    public class PassengerDetailsDto
    {
        public int Id { get; set; }
        public byte[]? ProfilePic { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string? City { get; set; }
        public string? PassengerAddress { get; set; }
    }
}
