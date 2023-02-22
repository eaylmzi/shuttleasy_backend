using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models
{
    public partial class PickupPoint
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public int GeoPointId { get; set; }
    }
}
