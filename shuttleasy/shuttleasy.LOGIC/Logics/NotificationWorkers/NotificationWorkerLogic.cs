using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.EFRepositories.NotificationWorkers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.NotificationWorkers
{
    public class NotificationWorkerLogic : INotificationWorkerLogic
    {
        private INotificationWorkerRepository _notificationWorkerRepository;
        public NotificationWorkerLogic(INotificationWorkerRepository notificationWorkerRepository)
        {
            _notificationWorkerRepository = notificationWorkerRepository;
        }
    }
}
