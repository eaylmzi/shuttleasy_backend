using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Models.dto.Passengers.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.JoinTables.dto
{
    public class ShuttleDetailsDto
    {
        public Company CompanyDetails { get; set; } = null!;
        public ShuttleSession ShuttleSessionDeparture { get; set; } = null!;
        public ShuttleSession ShuttleSessionReturn { get; set; } = null!;
        public PassengerCommentDto PassengerComment { get; set; } = null!;

    }
}
