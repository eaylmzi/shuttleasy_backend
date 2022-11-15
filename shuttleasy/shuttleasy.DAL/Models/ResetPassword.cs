using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class ResetPassword
    {
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public Guid ResetKey { get; set; }
        public DateTime Date { get; set; }

        public virtual Passenger Email1 { get; set; } = null!;
        public virtual CompanyWorker EmailNavigation { get; set; } = null!;
        public virtual Passenger? PhoneNumber1 { get; set; }
        public virtual CompanyWorker? PhoneNumberNavigation { get; set; }
    }
}
