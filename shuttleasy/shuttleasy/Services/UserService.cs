using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.JwtToken;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.Driver;
using shuttleasy.LOGIC.Logics.PasswordReset;
using shuttleasy.Mail;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Passengers.dto;

namespace shuttleasy.Services
{
    public class UserService : IUserService
    {
        private readonly IPassengerLogic _passengerLogic;
        private readonly IDriverLogic _driverLogic;
        private readonly IPasswordResetLogic _passwordResetLogic;


        private readonly IPasswordEncryption _passwordEncryption;
        private readonly IMapper _mapper;
        private readonly IJwtTokenManager _jwtTokenManager;
        private readonly IConfiguration _configuration;
        private readonly IMailManager _mailManager;
        



        public UserService(IPassengerLogic passengerLogic, IPasswordEncryption passwordEncryption, IMapper mapper,
            IJwtTokenManager jwtTokenManager, IConfiguration configuration, IDriverLogic driverLogic,
            IMailManager mailManager,IPasswordResetLogic passwordResetLogic)
        {//mailManager null olabilir diyo amk
            _passengerLogic = passengerLogic;
            _passwordEncryption = passwordEncryption;
            _mapper = mapper;
            _configuration = configuration;
            _jwtTokenManager = jwtTokenManager;
            _driverLogic = driverLogic;
            _mailManager = mailManager;
            _passwordResetLogic = passwordResetLogic;
        }
        public bool LoginPassenger(string email, string password)
        {          
            Passenger passenger = _passengerLogic.GetPassengerWithEmail(email);

            bool isMatched = _passwordEncryption.VerifyPasswordHash(passenger.PasswordHash,passenger.PasswordSalt, password);
            if (isMatched && passenger != null)
            {
                return true;
            }
            return false;
            //"1JCG6eSVTO"
        }
        public bool LoginDriver(string email, string password)
        {
            CompanyWorker companyWorker = _driverLogic.GetCompanyWorkerWithEmail(email);

            bool isMatched = _passwordEncryption.VerifyPasswordHash(companyWorker.PasswordHash,companyWorker.PasswordSalt, password);
            if (isMatched && companyWorker != null)
            {
                return true;
            }
            return false;
            //"1JCG6eSVTO"
        }
       /* public bool LoginAdmin(string email, string password) // Buraya bakılacak
        {        
            Passenger passenger = _passengerLogic.GetPassengerWithEmail(email);

            bool isMatched = _passwordEncryption.VerifyPasswordHash(passenger, password);
            if (isMatched && passenger != null)
            {
                return true;
            }
            return false;
            //"1JCG6eSVTO"
        }*/
        public Passenger SignUp(PassengerRegisterDto passengerRegisterDto,string role)
        {
            Passenger newPassenger = new Passenger();
            try
            {
                newPassenger = _mapper.Map<Passenger>(passengerRegisterDto);
                newPassenger.QrString = Guid.NewGuid();
                newPassenger.IsPayment = false;
                if (!string.IsNullOrEmpty(passengerRegisterDto.Password))
                {
                    _passwordEncryption.CreatePasswordHash(passengerRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);


                    newPassenger.PasswordHash = passwordHash;
                    newPassenger.PasswordSalt = passwordSalt;
                    newPassenger.Verified = true;

                    string token = _jwtTokenManager.CreateToken(newPassenger, role, _configuration);
                    newPassenger.Token = token;
                }
                else
                {
                    newPassenger.Verified = false;
                }

                bool result = _passengerLogic.Add(newPassenger);
                return newPassenger;
            }
            catch (Exception)
            { //Kendi kendiliğine 401 403 atıyo ama döndürmek istersek nasıl olacak
                return new Passenger(); //Bİr sıkıntı var 400 401 403 dönmek istediği zaman ne yapacam
            }


        }
        public CompanyWorker SignUp(DriverRegisterDto driverRegisterDto, string role)
        {
            CompanyWorker newCompanyWorker = new CompanyWorker();
            try
            {
                
                newCompanyWorker = _mapper.Map<CompanyWorker>(driverRegisterDto);
                string randomPassword = GetRandomString(10);

                _passwordEncryption.CreatePasswordHash(randomPassword, out byte[] passwordHash, out byte[] passwordSalt);
                newCompanyWorker.PasswordHash = passwordHash;
                newCompanyWorker.PasswordSalt = passwordSalt;

                newCompanyWorker.Verified = true;
                
                string token = _jwtTokenManager.CreateToken(newCompanyWorker, role, _configuration);
                newCompanyWorker.Token = token;

                bool result = _driverLogic.Add(newCompanyWorker);
                return newCompanyWorker;
            }
            catch (Exception)
            { //Kendi kendiliğine 401 403 atıyo ama döndürmek istersek nasıl olacak
                return new CompanyWorker(); //Bİr sıkıntı var 400 401 403 dönmek istediği zaman ne yapacam
            }


        }





        public void sendOTP(string email)
        {
            string otp = GetRandomOTP(6);
            _mailManager.sendMail(email,"Forgot your password ??",otp,_configuration);

        }
        public void ValidateOTP()
        {

        }




        private static Random random = new Random();

        public IMailManager MailManager { get; }

        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetRandomOTP(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
