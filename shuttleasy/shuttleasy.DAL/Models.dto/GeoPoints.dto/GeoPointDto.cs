using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.GeoPoints.dto
{
    public class GeoPointDto
    {
        public string Latitude { get; set; } = null!;
        public string Longtitude { get; set; } =null !;
        public string? LocationName { get; set; } 
    }
}
