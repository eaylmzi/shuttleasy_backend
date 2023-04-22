using shuttleasy.DAL.EFRepositories.ShuttleSessions;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.ShuttleBuses.dto;
using shuttleasy.DAL.Models.dto.ShuttleSessions.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.ShuttleSessions
{
    public class ShuttleSessionLogic : IShuttleSessionLogic
    {

        private IShuttleSessionRepository _shuttleSessionRepository;
        public ShuttleSessionLogic(IShuttleSessionRepository shuttleSessionRepository)
        {
            _shuttleSessionRepository = shuttleSessionRepository;
        }

        public ShuttleSession GetShuttleSessionWithCompanyId(int id) 
        {
            Func<ShuttleSession, bool> getShuttleSession = getShuttleSession => getShuttleSession.CompanyId == id;
            ShuttleSession shuttleSession = _shuttleSessionRepository.GetSingle(getShuttleSession) ?? throw new ArgumentNullException();
            return shuttleSession;
        }
        public bool CreateShuttleSession(ShuttleSession shuttleSession)
        {
            bool isAdded = _shuttleSessionRepository.Add(shuttleSession);
            return isAdded;
        }

        public bool DeleteShuttleSession(int shuttleSessionNumber)
        {
            Func<ShuttleSession, bool> getShuttleSession = getShuttleSession => getShuttleSession.Id == shuttleSessionNumber;
            bool isDeleted = _shuttleSessionRepository.Delete(getShuttleSession);
            return isDeleted;
        }
        /*
        public List<ShuttleSession>? FindSessionsWithSpecificLocation(int destinationNumber)
        {
            
            Func<ShuttleSession, bool> getShuttleSession = getShuttleSession => getShuttleSession.DestinationId == destinationNumber;
            List<ShuttleSession>? shuttleSessionList = _shuttleSessionRepository.Get(getShuttleSession);
            return shuttleSessionList;
        }*/

        public List<ShuttleSession>? GetAllSessionsWithCompanyId(int companyId)
        {
            Func<ShuttleSession, bool> getShuttleSession = getShuttleSession => getShuttleSession.CompanyId == companyId;
            List<ShuttleSession>? shuttleSessionList = _shuttleSessionRepository.Get(getShuttleSession);
            return shuttleSessionList;
        }

        /*
        public List<Destination>? FindDestinationWithBeginningDestination(string lastDestination)
        {
            Func<Destination, bool> getDestinationNumber = getDestinationNumber => getDestinationNumber.LastDestination == lastDestination;
            List<Destination>? destination = _destinationRepository.Get(getDestinationNumber);
            return destination;

        }*/
        public List<ShuttleSession>? FindShuttleSessionWithDestinationName(string destinationName)
        {
            Func<ShuttleSession, bool> getShuttleSession = getShuttleSession => getShuttleSession.DestinationName == destinationName;
            List<ShuttleSession>? shuttleSessions = _shuttleSessionRepository.Get(getShuttleSession);
            return shuttleSessions ;

        }
        public ShuttleSession? FindShuttleSessionById(int sessionNumber)
        {
            Func<ShuttleSession, bool> getSessionNumber = getSessionNumber => getSessionNumber.Id == sessionNumber;
            ShuttleSession? shuttleSessions = _shuttleSessionRepository.GetSingle(getSessionNumber);
            return shuttleSessions;

        }

        public async Task<bool> UpdateAsync(int id, ShuttleSession updatedShuttleSession)
        {
            Func<ShuttleSession, bool> getCompanyNumber = getCompanyNumber => getCompanyNumber.Id == id;
            bool isUpdated = await _shuttleSessionRepository.UpdateAsync(getCompanyNumber, updatedShuttleSession);
            return isUpdated;
        }
        public int? AddReturnId(ShuttleSession shuttleSession)
        {
            int? shuttleId = _shuttleSessionRepository.AddReturnId(shuttleSession);
            return shuttleId;
        }
        public async Task<bool> CheckAllForeignKeysAndUniqueExistAsync(ShuttleSessionDto shuttleSessionDto)
        {
            bool isExist = await _shuttleSessionRepository.CheckAllForeignKeysAndUniqueExistAsync(shuttleSessionDto);
            return isExist;
        }

    }
}

