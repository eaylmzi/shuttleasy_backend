using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.Passengers.dto
{
    public class PassengerNotificationTokenDto
    {
        public int Id { get; set; }
        public string NotificationToken { get; set; } = null!;
    }
}
