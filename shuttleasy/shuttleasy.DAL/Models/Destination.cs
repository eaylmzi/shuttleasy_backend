using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class Destination
    {
        public int Id { get; set; }
        public int CityNumber { get; set; }
        public string BeginningDestination { get; set; } = null!;
        public string LastDestination { get; set; } = null!;
    }
}
