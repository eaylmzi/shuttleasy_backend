using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.Driver.dto
{
    public class DriversInfoDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public double RatingAvg { get; set; }
        public int RateCount { get; set; }
        public double WorkingHours { get; set; }
    }
}
