using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;

namespace shuttleasy.DAL.EFRepositories
{
    public class PassengerRepository : Repository<Passenger>, IPassengerRepository
    {
        ShuttleasyDBContext _context = new ShuttleasyDBContext();

        private DbSet<Passenger> passengerTable { get; set; }
        public PassengerRepository()
        {
            passengerTable = _context.Set<Passenger>();
        }
        public async Task<bool> IsPhoneNumberAndEmailExist(string email, string phoneNumber)
        {
            return await passengerTable.AnyAsync(entity => entity.Email == email) &&
                await passengerTable.AnyAsync(entity => entity.PhoneNumber == phoneNumber);
        }

        public bool isPaid(string id)
        {         
            throw new NotImplementedException();
        }
        
    }
}
