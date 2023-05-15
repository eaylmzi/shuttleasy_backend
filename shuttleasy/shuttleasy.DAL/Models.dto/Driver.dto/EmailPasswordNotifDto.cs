using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.Driver.dto
{
    public class EmailPasswordNotifDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? NotificationToken { get; set; }
    }
}
