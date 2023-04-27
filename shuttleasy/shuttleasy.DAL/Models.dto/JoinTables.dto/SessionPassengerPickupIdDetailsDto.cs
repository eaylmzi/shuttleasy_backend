using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.JoinTables.dto
{
    public class SessionPassengerPickupIdDetailsDto
    {
        public int UserId { get; set; }
        public string NotificationToken { get; set; } = null!;
        public string Latitude { get; set; } = null!;
        public string Longtitude { get; set; } = null!;
    }
}
