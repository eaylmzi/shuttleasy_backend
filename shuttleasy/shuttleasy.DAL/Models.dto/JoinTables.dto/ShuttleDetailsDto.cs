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
        public ShuttleSessionDetailsDto ShuttleSessionDeparture { get; set; } = null!;
        public ShuttleSessionDetailsDto ShuttleSessionReturn { get; set; } = null!;

    }
}
