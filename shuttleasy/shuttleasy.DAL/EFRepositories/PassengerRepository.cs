using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;

namespace shuttleasy.DAL.EFRepositories
{
    public class PassengerRepository : Repository<Passenger>, IPassengerRepository
    {

        public bool isPaid(string id)
        {         
            throw new NotImplementedException();
        }
        
    }
}
