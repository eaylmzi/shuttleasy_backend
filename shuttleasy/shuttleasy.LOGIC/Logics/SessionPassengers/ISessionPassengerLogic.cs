using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.SessionPassengers
{
    public interface ISessionPassengerLogic
    {
        public bool Add(SessionPassenger sessionPassenger);
        public bool Delete(int sessionPassengerNumber);
        public bool DeleteBySessionId(int sessionID);
    }
}
