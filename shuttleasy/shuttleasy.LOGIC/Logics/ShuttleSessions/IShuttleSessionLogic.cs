using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.ShuttleSessions
{
    public interface IShuttleSessionLogic
    {
        public bool CreateShuttleSession(ShuttleSession shuttleSession);
        public bool DeleteShuttleSession(int shuttleSessionNumber);
        public ShuttleSession GetShuttleSessionWithCompanyId(int id);
       // public List<ShuttleSession>? FindSessionsWithSpecificLocation(int destinationNumber);
        public List<ShuttleSession>? GetAllSessionsWithCompanyId(int companyId);
        public List<ShuttleSession>? FindShuttleSessionWithDestinationName(string destinationName);
        public ShuttleSession? FindShuttleSessionById(int sessionNumber);
        public Task<bool> UpdateAsync(int id, ShuttleSession updatedShuttleSession);
        public int? AddReturnId(ShuttleSession shuttleSession);
    }
}
