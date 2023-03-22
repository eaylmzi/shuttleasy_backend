using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.JoinTables.dto
{
    public class ShuttleDetailsGroupDto
    {
        public Company CompanyDetail { get; set; }
        public List<ShuttleSession> ShuttleSessionDeparture { get; set; }
        public List<ShuttleSession> ShuttleSessionReturn { get; set; }
    }
}
