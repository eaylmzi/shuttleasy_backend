using shuttleasy.DAL.EFRepositories.SessionPassengers;
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
    }
}
