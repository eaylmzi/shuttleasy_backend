using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.ShuttleBuses.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.ShuttleBuses
{
    public interface IShuttleBusLogic
    {
        public bool Add(ShuttleBus shuttleBus);
        public List<ShuttleBus>? GetAllShuttleBusesWithCompanyId(int companyId);
        public bool DeleteShuttleBus(int shuttleBusNumber);
        public string? GetBusLicensePlateWithBusId(int busId);
        public Task<bool> CheckAllForeignKeysAndUniqueExistAsync(ShuttleBusDto shuttleBusDto);
    }
}
