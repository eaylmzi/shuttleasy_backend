using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class NotificationPassenger
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public DateTime Date { get; set; }
        public string? NotificationType { get; set; }
        public string? Content { get; set; }

    }
}
