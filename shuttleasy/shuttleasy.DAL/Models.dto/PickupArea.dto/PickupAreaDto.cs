using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.PickupArea.dto
{
    public class PickupAreaDto
    {
        public int SessionId { get; set; }
        public string PolygonPoints { get; set; } = null!;
    }
}
