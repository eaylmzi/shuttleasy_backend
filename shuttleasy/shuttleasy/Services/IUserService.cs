using shuttleasy.DAL.Models;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Passengers.dto;

namespace shuttleasy.Services
{
    public interface IUserService
    {
        public bool LoginDriver(string email, string password);
        public bool LoginPassenger(string email, string password);
        public Passenger SignUp(PassengerRegisterDto passengerRegisterDto, string role);
        public CompanyWorker SignUp(DriverRegisterDto driverRegisterDto, string role);
        public void sendOTP(string email);
    }
}
