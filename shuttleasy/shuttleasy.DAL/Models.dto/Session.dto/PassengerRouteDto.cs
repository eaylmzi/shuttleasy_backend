using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.Session.dto
{
    public class PassengerRouteDto
    {
        public int UserId { get; set; }
        public string Latitude { get; set; } = null!;
        public string Longtitude { get; set; } = null!;
        public string NotificationToken { get; set; } = null!;
    }
}
