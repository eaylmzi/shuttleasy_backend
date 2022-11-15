using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Interfaces
{
    public interface IPassengerRepository : IRepository<Passenger>
    {
        public Passenger GetPassenger(string id);
        public bool isPaid(string id);
        public Passenger GetPassengerWithEmail(string email);
        public bool AddPassenger(Passenger passenger);
    }
}
