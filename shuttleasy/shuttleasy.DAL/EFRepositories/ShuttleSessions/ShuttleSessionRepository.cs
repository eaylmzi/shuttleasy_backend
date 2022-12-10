using shuttleasy.DAL.EFRepositories.ShuttleBuses;
using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.ShuttleSessions
{
    public class ShuttleSessionRepository : Repository<ShuttleSession>, IShuttleSessionRepository
    {
    }
}
