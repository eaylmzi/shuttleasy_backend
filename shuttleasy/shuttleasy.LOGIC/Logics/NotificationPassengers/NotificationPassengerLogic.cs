using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.EFRepositories.NotificationPassengers;
using shuttleasy.DAL.EFRepositories.SessionHistories;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.NotificationPassengers
{
    public class NotificationPassengerLogic : INotificationPassengerLogic
    {
        private INotificationPassengerRepository _notificationPassengerRepository;
        public NotificationPassengerLogic(INotificationPassengerRepository notificationPassengerRepository)
        {
            _notificationPassengerRepository = notificationPassengerRepository;
        }
        public bool Add(NotificationPassenger notificationPassenger)
        {
            bool isAdded = _notificationPassengerRepository.Add(notificationPassenger);
            return isAdded;
        }

    }
}
