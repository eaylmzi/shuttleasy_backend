using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models
{
    public partial class PickupArea
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string PolygonPoints { get; set; } = null!;
    }
}
