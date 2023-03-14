using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.EFRepositories.NotificationPassengers;
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
    }
}
