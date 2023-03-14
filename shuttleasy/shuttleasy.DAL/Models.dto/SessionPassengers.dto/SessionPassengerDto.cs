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
        public DateTime SessionDate { get; set; }
        public byte[] EstimatedPickupTime { get; set; } = null!;
        public int PickupOrderNum { get; set; }
        public string PickupState { get; set; } = null!;
        public int PickupId { get; set; }
    }
}
