using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.ShuttleBuses
{
    public interface IShuttleBusLogic
    {
        public bool Add(ShuttleBus shuttleBus);
        public bool DeleteShuttleBus(int shuttleBusNumber);
    }
}
