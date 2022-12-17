using Org.BouncyCastle.Bcpg;
using shuttleasy.DAL.Models;
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Passengers.dto;
using shuttleasy.Models.dto.User.dto;

namespace shuttleasy.Services
{
    public interface IUserService
    {
        public bool LoginCompanyWorker(string email, string password);
        public bool LoginPassenger(string email, string password);
        public Passenger? SignUp(PassengerRegisterDto passengerRegisterDto, string role);
        public Passenger? CreatePassenger(PassengerRegisterPanelDto passengerRegisterPanelDto, string role);
        public CompanyWorker? CreateCompanyWorker(CompanyWorkerRegisterDto driverRegisterDto, string role);
        public ResetPassword? SendOTP(string email);
        public EmailTokenDto? ValidateOTP(string email, string otp);
        public object? resetPassword(string email, string password);
        public Passenger? UpdatePassengerProfile(Passenger passenger, UserProfileDto userProfileDto);

        public CompanyWorker? UpdateDriverProfile(CompanyWorker companyWorker, DriverProfileDto driverProfileDto);
        public bool CheckEmail(string email);

    }
}
