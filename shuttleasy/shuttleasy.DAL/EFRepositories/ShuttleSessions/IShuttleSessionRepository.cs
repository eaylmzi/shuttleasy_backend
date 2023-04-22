using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.ShuttleSessions.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.ShuttleSessions
{
    public interface IShuttleSessionRepository : IRepository<ShuttleSession>
    {
        public Task<bool> CheckAllForeignKeysAndUniqueExistAsync(ShuttleSessionDto shuttleSessionDto);
    }
}
