using shuttleasy.DAL.EFRepositories.PassengerRatings;
using shuttleasy.DAL.EFRepositories.SessionHistories;
using shuttleasy.DAL.EFRepositories.SessionPassengers;
using shuttleasy.DAL.Models;
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

        public bool Add(SessionHistory sessionHistory)
        {
            bool isAdded = _sessionHistoryRepository.Add(sessionHistory);
            return isAdded;
        }
        public bool Delete(int sessionHistoryNumber)
        {
            Func<SessionHistory, bool> getSessionHistoryNumber = getSessionHistoryNumber => getSessionHistoryNumber.Id == sessionHistoryNumber;
            bool isDeleted = _sessionHistoryRepository.Delete(getSessionHistoryNumber);
            return isDeleted;
        }
        public SessionHistory? GetSingleBySessionId(int sessionId)
        {
            Func<SessionHistory, bool> getSessionHistoryNumber = getSessionHistoryNumber => getSessionHistoryNumber.SessionId == sessionId;
            SessionHistory? sessionHistory = _sessionHistoryRepository.GetSingle(getSessionHistoryNumber);
            return sessionHistory;
        }
        public async Task<bool> UpdateAsync(SessionHistory updatedSessionHistory, int sessionId)
        {
            Func<SessionHistory, bool> getSessionHistory = getSessionHistory => getSessionHistory.SessionId == sessionId;
            bool isUpdated = await _sessionHistoryRepository.UpdateAsync(getSessionHistory, updatedSessionHistory);
            return isUpdated;
        }

    }
}
