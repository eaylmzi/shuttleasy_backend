using shuttleasy.DAL.EFRepositories.SessionPassengers;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.SessionPassengers
{
    public class SessionPassengerLogic : ISessionPassengerLogic
    {
        private ISessionPassengerRepository _sessionPassengerRepository;
        public SessionPassengerLogic(ISessionPassengerRepository sessionPassengerRepository)
        {
            _sessionPassengerRepository = sessionPassengerRepository;
        }


        public bool Add(SessionPassenger sessionPassenger)
        {
            bool isAdded = _sessionPassengerRepository.Add(sessionPassenger);
            return isAdded;
        }
        public bool Delete(int sessionPassengerNumber)
        {
            Func<SessionPassenger, bool> getSessionPassengerNumber = getSessionPassengerNumber => getSessionPassengerNumber.Id == sessionPassengerNumber;
            bool isDeleted = _sessionPassengerRepository.Delete(getSessionPassengerNumber);
            return isDeleted;
        }
        public bool DeleteBySessionId(int sessionID)
        {
            Func<SessionPassenger, bool> getSessionPassenger = getSessionPassenger => getSessionPassenger.SessionId == sessionID;
            bool isDeleted = _sessionPassengerRepository.DeleteList(getSessionPassenger);
            return isDeleted;
        }
        public List<SessionPassenger>? GetListById(int sessionNumber)
        {
            Func<SessionPassenger, bool> getSessionNumber = getSessionNumber => getSessionNumber.SessionId == sessionNumber;
            List<SessionPassenger>? sessionPassenger = _sessionPassengerRepository.Get(getSessionNumber);
            return sessionPassenger;
        }
        public SessionPassenger? Get(int id, int sessionId)
        {
            Func<SessionPassenger, bool> getSessionNumber = getSessionNumber => getSessionNumber.Id == id;
            Func<SessionPassenger, bool> getShuttle = getShuttle => getShuttle.SessionId == sessionId;
            SessionPassenger? sessionPassenger = _sessionPassengerRepository.GetSingle(getSessionNumber, getShuttle);
            return sessionPassenger;
        }
        public async Task<bool> UpdateAsync(int id, SessionPassenger updatedSessionPassenger)
        {
            Func<SessionPassenger, bool> getSessionPassenger = getSessionPassenger => getSessionPassenger.Id == id;
            bool isUpdated = await _sessionPassengerRepository.UpdateAsync(getSessionPassenger, updatedSessionPassenger);
            return isUpdated;
        }


    }
}
