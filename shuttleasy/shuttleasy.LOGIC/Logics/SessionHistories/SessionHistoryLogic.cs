using shuttleasy.DAL.EFRepositories.PassengerRatings;
using shuttleasy.DAL.EFRepositories.SessionHistories;
using shuttleasy.DAL.EFRepositories.SessionPassengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.SessionHistories
{
    public class SessionHistoryLogic : ISessionHistoryLogic
    {
        private ISessionHistoryRepository _sessionHistoryRepository;
        public SessionHistoryLogic(ISessionHistoryRepository sessionHistoryRepository)
        {
            _sessionHistoryRepository = sessionHistoryRepository;
        }
    }
}
