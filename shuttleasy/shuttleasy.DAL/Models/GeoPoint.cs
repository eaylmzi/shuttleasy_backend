using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models
{
    public partial class GeoPoint
    {
        public int Id { get; set; }
        public string Latitude { get; set; } = null!;
        public string Longtitude { get; set; } = null!;
        public string LocationName { get; set; } = null!;
    }
}
