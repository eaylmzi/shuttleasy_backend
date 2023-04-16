using Org.BouncyCastle.Bcpg;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.DAL.Models.dto.Passengers.dto;
using shuttleasy.DAL.Models.dto.User.dto;

namespace shuttleasy.Services
{
    public interface IUserService
    {
        public CompanyWorker? LoginCompanyWorker(string email, string password);
        public Passenger? LoginPassenger(string email, string password);
        public Passenger? SignUp(PassengerRegisterDto passengerRegisterDto, string role);
        public Passenger? CreatePassenger(PassengerRegisterPanelDto passengerRegisterPanelDto, string role);
        public CompanyWorker? CreateCompanyWorker(CompanyWorkerRegisterDto driverRegisterDto, string role);
        public DateTime? SendOTP(string email);
        public EmailTokenDto? ValidateOTP(string email, string otp);
        public object? resetPassword(string email, string password);
        public Passenger? UpdatePassengerProfile(Passenger passenger, UserProfileDto userProfileDto);

        public CompanyWorker? UpdateDriverProfile(CompanyWorker companyWorker, DriverProfileDto driverProfileDto);
        public bool CheckEmailandPhoneNumberForPassengers(string email, string phoneNumber);
        public bool CheckEmailandPhoneNumberForCompanyWorker(string email,string phoneNumber);
        public bool CheckEmail(string email);
        public bool VerifyUser(UserVerifyingDto userInformation);
        public bool calculateRating(int sessionId, double rating);

    }
}
