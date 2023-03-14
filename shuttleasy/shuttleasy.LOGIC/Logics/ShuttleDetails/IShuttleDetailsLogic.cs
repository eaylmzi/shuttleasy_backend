using shuttleasy.DAL.Models.dto.ShuttleDetails.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.ShuttleDetails
{
    public interface IShuttleDetailsLogic
    {
        public List<ShuttleDetailsDto> InnerJoinTables(int companyId);
    }
}
