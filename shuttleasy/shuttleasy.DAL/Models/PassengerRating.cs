using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class PassengerRating
    {
        public string PassengerIdentityNum { get; set; } = null!;
        public int Rating { get; set; }
        public int SessionId { get; set; }
        public DateTime Date { get; set; }
        public string? Comment { get; set; }

        public virtual Passenger PassengerIdentityNumNavigation { get; set; } = null!;
        public virtual ShuttleSession Session { get; set; } = null!;
    }
}
