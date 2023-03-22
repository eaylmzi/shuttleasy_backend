using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.Passengers.dto
{
    public class PassengerCommentDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Comments { get; set; } = null!;
        public DateTime Date { get; set; }
        public byte[]? ProfilePic { get; set; }
        public int Rating { get; set; }
    }
}
