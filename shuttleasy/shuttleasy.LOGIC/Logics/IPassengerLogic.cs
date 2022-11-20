using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics
{
    public interface IPassengerLogic
    {
        public bool Add(Passenger passenger);
        public List<Passenger> Get();
        public Passenger GetPassengerWithEmail(string email);
        public Passenger GetPassengerWithId(string id);
    }
}
