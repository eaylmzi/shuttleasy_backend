using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.JoinTables.dto
{
    public class CommentDetailsDto
    {
        public int PassengerIdentity { get; set; }
        public int Rating { get; set; }
        public int SessionId { get; set; }
        public DateTime Date { get; set; }
        public string? Comment { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public byte[]? ProfilePic { get; set; }
    }
}
