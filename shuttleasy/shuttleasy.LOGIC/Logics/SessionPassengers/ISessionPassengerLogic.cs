using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.SessionPassengers
{
    public interface ISessionPassengerLogic
    {
        public bool Add(SessionPassenger sessionPassenger);
        public bool Delete(int sessionPassengerNumber);
        public bool DeleteBySessionId(int sessionID);
        public SessionPassenger? Get(int id, int sessionId);
        public List<SessionPassenger>? GetListById(int sessionNumber);
        public Task<bool> UpdateAsync(int id, SessionPassenger updatedSessionPassenger);
    }
}
