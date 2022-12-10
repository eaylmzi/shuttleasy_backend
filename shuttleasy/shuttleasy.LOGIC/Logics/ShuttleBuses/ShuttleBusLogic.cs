using shuttleasy.DAL.EFRepositories.PasswordReset;
using shuttleasy.DAL.EFRepositories.ShuttleBuses;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.ShuttleBuses
{
    public class ShuttleBusLogic : IShuttleBusLogic
    {
        private readonly IShuttleBusRepository _shuttleBusRepository;

        public ShuttleBusLogic(IShuttleBusRepository shuttleBusRepository)
        {
            _shuttleBusRepository = shuttleBusRepository;        
        }


        public bool Add(ShuttleBus shuttleBus)
        {
            bool isAdded = _shuttleBusRepository.Add(shuttleBus);
            return isAdded;
        }
        public bool DeleteShuttleBus(int shuttleBusNumber) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<ShuttleBus, bool> getShuttleBus = getShuttleBus => getShuttleBus.Id == shuttleBusNumber;
            bool isDeleted = _shuttleBusRepository.Delete(getShuttleBus);
            return isDeleted;
        }
    }
}
