﻿using shuttleasy.DAL.EFRepositories.Destinations;
using shuttleasy.DAL.EFRepositories.ShuttleSessions;
using shuttleasy.DAL.Models;
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

        public List<ShuttleSession>? FindSessionWithSpecificLocation(int destinationNumber)
        {
            Func<ShuttleSession, bool> getShuttleSession = getShuttleSession => getShuttleSession.BusId == destinationNumber;
            List<ShuttleSession>? shuttleSessionList = _shuttleSessionRepository.Get(getShuttleSession);
            return shuttleSessionList;
        }

    }
}

