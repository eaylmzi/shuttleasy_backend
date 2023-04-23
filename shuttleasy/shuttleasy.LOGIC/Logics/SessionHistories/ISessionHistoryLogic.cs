using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.SessionHistories
{
    public interface ISessionHistoryLogic
    {
        public bool Add(SessionHistory sessionHistory);
        public bool Delete(int sessionHistoryNumber);
        public SessionHistory GetSingleBySessionId(int sessionId);
        public Task<bool> UpdateAsync(SessionHistory updatedSessionHistory, int sessionId);
    }
}
