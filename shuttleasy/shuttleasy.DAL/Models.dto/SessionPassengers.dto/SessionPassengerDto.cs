using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.SessionPassengers.dto
{
    public class SessionPassengerDto
    {
        public int SessionId { get; set; }
        public string Longitude { get; set; } = null!;
        public string Latitude { get; set; } = null!;

    }
}
