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
        public bool isPaid(string id);
        public Passenger? GetPassengerWithEmail(string email);
        public Passenger? GetPassengerWithId(string id);
    }
}
