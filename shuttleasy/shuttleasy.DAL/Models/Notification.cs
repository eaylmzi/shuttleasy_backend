using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class Notification
    {
        public string Email { get; set; } = null!;
        public DateTime Date { get; set; }
        public string? NotificationType { get; set; }
        public string? Content { get; set; }

        public virtual Passenger Email1 { get; set; } = null!;
        public virtual CompanyWorker EmailNavigation { get; set; } = null!;
    }
}
