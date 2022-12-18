using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities;
using shuttleasy.DAL.EFRepositories;
using shuttleasy.DAL.EFRepositories.CompanyWorkers;
using shuttleasy.DAL.EFRepositories.PasswordReset;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.Encryption;
using shuttleasy.JwtToken;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.PasswordReset;
using shuttleasy.Mail;
using shuttleasy.Models;
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.Models.dto.Driver.dto;
using shuttleasy.Models.dto.Passengers.dto;
using shuttleasy.Models.dto.User.dto;
using System;
using System.Data;

namespace shuttleasy.Services
{
    public class UserService : IUserService
    {
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _driverLogic;
        private readonly IPasswordResetLogic _passwordResetLogic;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly ICompanyWorkerRepository _driverRepository;
        private readonly IPasswordEncryption _passwordEncryption;
        private readonly IMapper _mapper;
        private readonly IJwtTokenManager _jwtTokenManager;
        private readonly IConfiguration _configuration;
        private readonly IMailManager _mailManager;
        private readonly IPassengerRepository _passengerRepository;




        public UserService(IPassengerLogic passengerLogic, IPasswordEncryption passwordEncryption, IMapper mapper,
            IJwtTokenManager jwtTokenManager, IConfiguration configuration, ICompanyWorkerLogic driverLogic,
            IMailManager mailManager,IPasswordResetLogic passwordResetLogic,IPasswordResetRepository passwordResetRepository,
            ICompanyWorkerRepository driverRepository,IPassengerRepository passengerRepository)
        {//mailManager null olabilir diyo amk
            _passengerLogic = passengerLogic;
            _passwordEncryption = passwordEncryption;
            _mapper = mapper;
            _configuration = configuration;
            _jwtTokenManager = jwtTokenManager;
            _driverLogic = driverLogic;
            _mailManager = mailManager;
            _passwordResetLogic = passwordResetLogic;
            _passwordResetRepository = passwordResetRepository;
            _driverRepository = driverRepository;
            _passengerRepository = passengerRepository;
        }
        public bool LoginPassenger(string email, string password)
        {          
            Passenger passenger = _passengerLogic.GetPassengerWithEmail(email)
                    ?? throw new ArgumentNullException();

            bool isMatched = _passwordEncryption.VerifyPasswordHash(passenger.PasswordHash,passenger.PasswordSalt, password);
            if (isMatched && passenger != null)
            {
                return true;
            }
            return false;
            //"1JCG6eSVTO"
        }
        public bool LoginCompanyWorker(string email, string password)
        {
            CompanyWorker companyWorker = _driverLogic.GetCompanyWorkerWithEmail(email)
                    ??throw new ArgumentNullException();

            bool isMatched = _passwordEncryption.VerifyPasswordHash(companyWorker.PasswordHash,companyWorker.PasswordSalt, password);
            if (isMatched && companyWorker != null)
            {
                return true;
            }
            return false;
        }
        public Passenger? SignUp(PassengerRegisterDto passengerRegisterDto,string role)
        {
            Passenger newPassenger = new Passenger();
            newPassenger = _mapper.Map<Passenger>(passengerRegisterDto);
            newPassenger.QrString = Guid.NewGuid();

            if (!string.IsNullOrEmpty(passengerRegisterDto.Password))
            {
                _passwordEncryption.CreatePasswordHash(passengerRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                newPassenger.PasswordHash = passwordHash;
                newPassenger.PasswordSalt = passwordSalt;
                newPassenger.Verified = true;
            }
            else
            {
                newPassenger.Verified = false;
            }

            _passengerLogic.Add(newPassenger);

            Passenger? passengerFromDB = _passengerLogic.GetPassengerWithEmail(newPassenger.Email);
            if(passengerFromDB != null)
            {
                string token = _jwtTokenManager.CreateToken(passengerFromDB, role, _configuration);
                passengerFromDB.Token = token;
                _passengerLogic.UpdatePassengerWithEmail(passengerFromDB, passengerFromDB.Email);
                return passengerFromDB;
            }

            return null;
            

        }
        public Passenger? CreatePassenger(PassengerRegisterPanelDto passengerRegisterPanelDto, string role)
        {
            Passenger newPassenger = new Passenger();
            newPassenger = _mapper.Map<Passenger>(passengerRegisterPanelDto);
            string randomPassword = GetRandomString(10);
            _passwordEncryption.CreatePasswordHash(randomPassword, out byte[] passwordHash, out byte[] passwordSalt);
            newPassenger.PasswordHash = passwordHash;
            newPassenger.PasswordSalt = passwordSalt;

            newPassenger.Verified = true;

            _passengerLogic.Add(newPassenger);
            Passenger? passengerFromDB = _passengerLogic.GetPassengerWithEmail(newPassenger.Email);
            if (passengerFromDB != null)
            {
                string token = _jwtTokenManager.CreateToken(passengerFromDB, role, _configuration);
                passengerFromDB.Token = token;
                _passengerLogic.UpdatePassengerWithEmail(passengerFromDB, passengerFromDB.Email);
                return passengerFromDB;
            }
            return null;

        }
        public CompanyWorker? CreateCompanyWorker(CompanyWorkerRegisterDto driverRegisterDto, string role)
        {
            CompanyWorker newCompanyWorker = new CompanyWorker();
            newCompanyWorker = _mapper.Map<CompanyWorker>(driverRegisterDto);
            string randomPassword = GetRandomString(10);
            _passwordEncryption.CreatePasswordHash(randomPassword, out byte[] passwordHash, out byte[] passwordSalt);
            newCompanyWorker.PasswordHash = passwordHash;
            newCompanyWorker.PasswordSalt = passwordSalt;
            
            newCompanyWorker.Verified = true;
            newCompanyWorker.WorkerType = role;


            _driverLogic.Add(newCompanyWorker);
            CompanyWorker? companyWorkerFromDB = _driverLogic.GetCompanyWorkerWithEmail(newCompanyWorker.Email);
            if (companyWorkerFromDB != null)
            {
                string token = _jwtTokenManager.CreateToken(companyWorkerFromDB, role, _configuration);
                companyWorkerFromDB.Token = token;
                _driverLogic.UpdateCompanyWorkerWithEmail(companyWorkerFromDB, companyWorkerFromDB.Email);
                return companyWorkerFromDB;
            }
            return null;

        }



        public CompanyWorker? UpdateDriverProfile(CompanyWorker companyWorker, DriverProfileDto driverProfileDto)
        {

             CompanyWorker updatedDriver = companyWorker;

             updatedDriver.ProfilePic = setProfilePhoto(driverProfileDto.ProfilePic);
             updatedDriver.Name = driverProfileDto.Name;
             updatedDriver.Surname = driverProfileDto.Surname;
             updatedDriver.Email = driverProfileDto.Email;
             updatedDriver.PhoneNumber = setPhoneNumber(driverProfileDto.PhoneNumber);

            bool isUpdated = _driverLogic.UpdateCompanyWorkerWithEmail(updatedDriver, companyWorker.Email);
            if (isUpdated)
            {
                return updatedDriver;
            }


            return null;
        }
        public Passenger? UpdatePassengerProfile(Passenger passenger, UserProfileDto userprofileDto)
        {

            Passenger updatedPassenger = passenger;

            updatedPassenger.ProfilePic = setProfilePhoto(userprofileDto.ProfilePic);
            updatedPassenger.Name = userprofileDto.Name;
            updatedPassenger.Surname = userprofileDto.Surname;
            updatedPassenger.Email = userprofileDto.Email;
            updatedPassenger.PhoneNumber = setPhoneNumber(userprofileDto.PhoneNumber);
            updatedPassenger.City= userprofileDto.City;
            updatedPassenger.PassengerAddress = setAddress(userprofileDto.PassengerAddress); 

            bool isUpdated = _passengerLogic.UpdatePassengerWithEmail(updatedPassenger, passenger.Email);
            if (isUpdated)
            {
                return updatedPassenger;
            }


            return null;
        }
        private byte[]? setProfilePhoto(byte[]? profilePhoto)
        {
            if (profilePhoto.IsNullOrEmpty())
            {
                return null;
            }
            return profilePhoto;

        }
        private string? setPhoneNumber(string? phoneNumber)
        {
            if (phoneNumber.IsNullOrEmpty())
            {
                return null;
            }
            return phoneNumber;
        }
        private string? setAddress(string? address)
        {
            if (address.IsNullOrEmpty())
            {
                return null;
            }
            return address;
        }

        public bool CheckEmail(string email)
        {
            Passenger? passenger = _passengerLogic.GetPassengerWithEmail(email);
            if (passenger != null)
            {
                return true;
            }
            else
            {
                CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithEmail(email);
                if (companyWorker != null)
                {
                    return true;
                }
                else
                {

                    return false;
                }

            }

        }



        public DateTime? SendOTP(string email)
        {
            Passenger? passenger = _passengerLogic.GetPassengerWithEmail(email);
            if (passenger != null)
            {
                if (!isAnyValidOTP(email))
                {
                    string otp = GetRandomOTP(6);
                    // _mailManager.sendMail(email, "Password Reset Request", otp,_configuration);
                    ResetPassword resetPassword = new ResetPassword();
                    resetPassword.Email = email;
                    resetPassword.Date = DateTime.Now;
                    resetPassword.ResetKey = otp;
                    DateTime expireDate = DateTime.Now.AddSeconds(180);
                    _passwordResetLogic.Add(resetPassword);
                    return expireDate;

                }
                return null;

            }
            else
            {
                CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithEmail(email);
                if (companyWorker != null)
                {
                    if (!isAnyValidOTP(email))
                    {
                        string otp = GetRandomOTP(6);
                        // _mailManager.sendMail(email, "Password Reset Request", otp,_configuration);
                        ResetPassword resetPassword = new ResetPassword();
                        resetPassword.Email = email;
                        resetPassword.Date = DateTime.Now;
                        resetPassword.ResetKey = otp;
                        DateTime expireDate = DateTime.Now.AddSeconds(180);
                        _passwordResetLogic.Add(resetPassword);
                        return expireDate;

                    }
                }
                return null;
            }
            
        }
        public EmailTokenDto? ValidateOTP(string email,string otp)
        {
            ResetPassword resetPassword = _passwordResetLogic.GetResetPasswordWithEmail(email)
                 ?? throw new ArgumentNullException();
            Passenger? passenger;
            CompanyWorker? companyWorker;

            if (resetPassword != null)
            {
                if (resetPassword.ResetKey.Equals(otp))
                {

                    int time = getTimeDiffereance(resetPassword.Date);
                    if (time < 180)
                    {
                        EmailTokenDto emailTokenDto;
                        passenger = _passengerLogic.GetPassengerWithEmail(email);
                        if (passenger!= null)
                        {
                            if(passenger.Token != null)
                            {
                                emailTokenDto = new EmailTokenDto();
                                emailTokenDto.Email = email;
                                emailTokenDto.Token = passenger.Token;
                                return emailTokenDto;

                            }
                            
                        }
                        else
                        {
                            companyWorker = _driverLogic.GetCompanyWorkerWithEmail(email);
                            if(companyWorker!= null)
                            {
                                if(companyWorker.Token != null)
                                {
                                    emailTokenDto = new EmailTokenDto();
                                    emailTokenDto.Email = email;
                                    emailTokenDto.Token = companyWorker.Token;
                                    return emailTokenDto;

                                }
                            }                          
                        }
                    }
                }
            }

            return null;

        }
        public object? resetPassword(string email,string password)
        {
            if (_passengerLogic.GetPassengerWithEmail(email) != null)
            {
                Passenger passenger = _passengerLogic.GetPassengerWithEmail(email)
                        ?? throw new ArgumentNullException();
                if (passenger.Verified == false)
                {
                    passenger.Verified = true;
                }
                _passwordEncryption.ResetPassengerPassword(password, passenger);

                bool isUpdated = _passengerLogic.UpdatePassengerWithEmail(passenger, email);
                if (isUpdated)
                {
                    PassengerInfoDto passengerInfoDto = _mapper.Map<PassengerInfoDto>(passenger);
                    return passengerInfoDto;
                }
                else
                {
                    return null;
                }
                
            }
            else
            {
                CompanyWorker companyWorker = _driverLogic.GetCompanyWorkerWithEmail(email)
                        ?? throw new ArgumentNullException();
                if (companyWorker.Verified == false)
                {
                    companyWorker.Verified = true;
                }
                _passwordEncryption.ResetDriverPassword(password, companyWorker);
                bool isUpdated = _driverLogic.UpdateCompanyWorkerWithEmail(companyWorker, email);
                if (isUpdated)
                {
                    CompanyWorkerInfoDto driverInfoDto = _mapper.Map<CompanyWorkerInfoDto>(companyWorker);
                    return driverInfoDto;
                }
                else
                {
                    return null;
                }
               
            }

        }

      
        private bool isAnyValidOTP(string email)
        {
            ResetPassword? resetPassword = _passwordResetLogic.GetResetPasswordWithEmail(email);
            if (resetPassword != null)
            {
                int time = getTimeDiffereance(resetPassword.Date);
                if (time > 180)
                {
                    _passwordResetLogic.DeleteResetPasswordWithEmail(resetPassword.Email);
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        private int getTimeDiffereance(DateTime dateTime)
        {
            var remainingTime = DateTime.Now - dateTime;
            int day = remainingTime.Days * 86400;
            int hours = remainingTime.Hours * 3600;
            int minutes = remainingTime.Minutes * 60;
            int seconds = remainingTime.Seconds;
            int time = day + hours + minutes + seconds;
            return time;
        }


        public bool CheckEmailandPhoneNumber(string email,string phoneNumber)
        {
            Passenger? forPassengerEmail = _passengerLogic.GetPassengerWithEmail(email);
            Passenger? forPassengerPhone = _passengerLogic.GetPassengerWithPhoneNumber(phoneNumber);
            if (forPassengerEmail != null || forPassengerPhone != null)
            {
                return true;
            }
            else
            {
                CompanyWorker? forCompanyWorkerEmail = _driverLogic.GetCompanyWorkerWithEmail(email);
                CompanyWorker? forCompanyWorkerPhone = _driverLogic.GetCompanyWorkerWithPhoneNumber(phoneNumber);
                if (forCompanyWorkerEmail != null || forCompanyWorkerPhone != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }

        }

        public bool VerifyUser(UserVerifyingDto userInformation)
        {
            if (Roles.Passenger.Equals(userInformation.Role))
            {
                Passenger? passenger = _passengerLogic.GetPassengerWithId(userInformation.Id);
                if (passenger != null)
                {
                    if(passenger.Token != null)
                    {
                        if (passenger.Token.Equals(userInformation.Token))
                        {
                            return true;
                        }

                    }
                    
                }
                return false;
            }
            else if (Roles.Driver.Equals(userInformation.Role) || Roles.Admin.Equals(userInformation.Role) || Roles.SuperAdmin.Equals(userInformation.Role))
            {
                CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithId(userInformation.Id);
                if (companyWorker != null)
                {
                    if (companyWorker.Token != null)
                    {
                        if (companyWorker.Token.Equals(userInformation.Token))
                        {
                            return true;
                        }

                    }

                }
                return false;
            }
            return false;
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
