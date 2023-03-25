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

    }
}
