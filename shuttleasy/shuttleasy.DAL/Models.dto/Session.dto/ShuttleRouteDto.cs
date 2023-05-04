using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.Session.dto
{
    public class ShuttleRouteDto
    {
        public int Id { get; set; }
        public GeoPoint StartGeopoint { get; set; } = null!;
        public GeoPoint FinalGeopoint { get; set; } = null!;
    }
}
