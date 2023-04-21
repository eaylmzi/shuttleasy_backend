using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.ShuttleBuses.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.ShuttleBuses
{
    public interface IShuttleBusRepository : IRepository<ShuttleBus>
    {
        public Task<bool> CheckAllForeignKeysAndUniqueExistAsync(ShuttleBusDto shuttleBusDto);
    }
}
