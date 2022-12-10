using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class PassengerRating
    {
        public int Id { get; set; }
        public int PassengerIdentity { get; set; }
        public int Rating { get; set; }
        public int SessionId { get; set; }
        public DateTime Date { get; set; }
        public string? Comment { get; set; }
    }
}
