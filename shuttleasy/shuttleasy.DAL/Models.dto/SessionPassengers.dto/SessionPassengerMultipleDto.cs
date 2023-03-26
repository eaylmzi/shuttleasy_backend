using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.SessionPassengers.dto
{
    public class SessionPassengerMultipleDto
    {
        public List<int> SessionIdList { get; set; } = null!;
        public string Longitude { get; set; } = null!;
        public string Latitude { get; set; } = null!;
    }
}
