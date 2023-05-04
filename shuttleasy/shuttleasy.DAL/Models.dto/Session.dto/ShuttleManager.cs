using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.Session.dto
{
    public class ShuttleManager
    {
        public List<PassengerRouteDto> PassengerRouteDto { get; set; } = null!;
        public ShuttleRouteDto ShuttleRouteDto { get; set; } = null!;

    }
}
