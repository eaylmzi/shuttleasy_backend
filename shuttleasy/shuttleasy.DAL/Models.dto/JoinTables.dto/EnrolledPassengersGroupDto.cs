using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.JoinTables.dto
{
    public class EnrolledPassengersGroupDto
    {
        public ShuttleSession? ShuttleSession { get; set; }
        public List<PassengerDetailsDto>? PassengerDetailsDtoList { get; set; }
    }
}
