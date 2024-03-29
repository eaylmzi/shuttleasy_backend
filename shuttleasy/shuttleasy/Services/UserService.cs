﻿using AutoMapper;
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
using shuttleasy.DAL.Models.dto.Credentials.dto;
using shuttleasy.DAL.Models.dto.Driver.dto;
using shuttleasy.DAL.Models.dto.Passengers.dto;
using shuttleasy.DAL.Models.dto.User.dto;
using System;
using System.Data;
using shuttleasy.LOGIC.Logics.Companies;
using shuttleasy.LOGIC.Logics.ShuttleSessions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using shuttleasy.LOGIC.Logics.SessionHistories;
using shuttleasy.LOGIC.Logics.DriversStatistics;
using shuttleasy.Services.NotifService;
using shuttleasy.DAL.Models.dto.Session.dto;
using shuttleasy.LOGIC.Logics.GeoPoints;
using shuttleasy.LOGIC.Logics.JoinTables;

namespace shuttleasy.Services
{
    public class UserService : IUserService
    {
        private readonly IPassengerLogic _passengerLogic;
        private readonly ICompanyWorkerLogic _companyWorkerLogic;
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
        private readonly ICompanyLogic _companyLogic;
        private readonly IShuttleSessionLogic _shuttleSessionLogic;
        private readonly ISessionHistoryLogic _sessionHistoryLogic;
        private readonly IDriversStatisticLogic _driversStatisticLogic;
        private readonly IGeoPointLogic _geoPointLogic;
        private readonly IJoinTableLogic _joinTableLogic;
        private static IWebHostEnvironment _webHostEnvironment;




        public UserService(IPassengerLogic passengerLogic, IPasswordEncryption passwordEncryption, IMapper mapper,
            IJwtTokenManager jwtTokenManager, IConfiguration configuration, ICompanyWorkerLogic driverLogic,
            IMailManager mailManager,IPasswordResetLogic passwordResetLogic,IPasswordResetRepository passwordResetRepository,
            ICompanyWorkerRepository driverRepository,IPassengerRepository passengerRepository, ICompanyLogic companyLogic,
            IShuttleSessionLogic shuttleSessionLogic, ICompanyWorkerLogic companyWorkerLogic, IWebHostEnvironment webHostEnvironment,
            ISessionHistoryLogic sessionHistoryLogic, IDriversStatisticLogic driversStatisticLogic, IGeoPointLogic geoPointLogic,
            IJoinTableLogic joinTableLogic)
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
            _companyLogic = companyLogic;
            _shuttleSessionLogic = shuttleSessionLogic;
            _companyWorkerLogic = companyWorkerLogic;
            _webHostEnvironment = webHostEnvironment;
            _sessionHistoryLogic = sessionHistoryLogic;
            _driversStatisticLogic = driversStatisticLogic;
            _geoPointLogic = geoPointLogic;
            _joinTableLogic = joinTableLogic;
        }

      

        public Passenger? LoginPassenger(string email, string password)
        {          
            Passenger? passenger = _passengerLogic.GetPassengerWithEmail(email);
            if(passenger != null)
            {
                bool isMatched = _passwordEncryption.VerifyPasswordHash(passenger.PasswordHash, passenger.PasswordSalt, password);
                if (isMatched && passenger != null)
                {
                    return passenger;
                }
            }
            return null;
            //"1JCG6eSVTO"
        }
        public CompanyWorker? LoginCompanyWorker(string email, string password)
        {
            CompanyWorker? companyWorker = _driverLogic.GetCompanyWorkerWithEmail(email);
            if(companyWorker != null)
            {
                bool isMatched = _passwordEncryption.VerifyPasswordHash(companyWorker.PasswordHash, companyWorker.PasswordSalt, password);
                if (isMatched && companyWorker != null)
                {
                    return companyWorker;
                }
            }
            return null;
        }


        private Passenger? AssingPassengerToken(int id, string role)
        {
            Passenger? passenger = _passengerLogic.GetSingle(id);
            if (passenger != null)
            {
                string token = _jwtTokenManager.CreateToken(passenger, role, _configuration);
                passenger.Token = token;
                _passengerLogic.UpdatePassengerWithEmail(passenger, passenger.Email);
                return passenger;
            }           
            return null;
        }
        private CompanyWorker? AssingCompanyWorkerToken(int id, string role)
        {
            CompanyWorker? companyWorker = _companyWorkerLogic.GetSingle(id);
            if (companyWorker != null)
            {
                string token = _jwtTokenManager.CreateToken(companyWorker, role, _configuration);
                companyWorker.Token = token;
                _companyWorkerLogic.UpdateCompanyWorkerWithEmail(companyWorker, companyWorker.Email);
                return companyWorker;
            }
            return null;
        }    
        private T? AssignToken<T>(int id, string role) where T : class
        {
            if (role == Roles.Passenger)
            {
                Passenger? passenger = AssingPassengerToken(id, role);
                if(passenger != null)
                {
                    return passenger as T;
                }           
            }
            else if (role == Roles.Driver)
            {
                CompanyWorker? companyWorker = AssingCompanyWorkerToken(id, role);
                if (companyWorker != null)
                {
                    return companyWorker as T;
                }
            }
            else if (role == Roles.Admin)
            {
                CompanyWorker? companyWorker = AssingCompanyWorkerToken(id, role);
                if (companyWorker != null)
                {
                    return companyWorker as T;
                }
            }
            else if (role == Roles.SuperAdmin)
            {
                CompanyWorker? companyWorker = AssingCompanyWorkerToken(id, role);
                if (companyWorker != null)
                {
                    return companyWorker as T;
                }
            }
            return null;

        }
        public Passenger? SignUp(PassengerRegisterDto passengerRegisterDto,string role)
        {
            Passenger newPassenger = new Passenger();
            newPassenger = _mapper.Map<Passenger>(passengerRegisterDto);
           

            if (string.IsNullOrEmpty(passengerRegisterDto.Password))
            {
                return null;
            }

            _passwordEncryption.CreatePasswordHash(passengerRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            newPassenger.PasswordHash = passwordHash;
            newPassenger.PasswordSalt = passwordSalt;
            newPassenger.QrString = Guid.NewGuid();
            newPassenger.Verified = true;
            int id = _passengerLogic.AddReturnId(newPassenger);

            Passenger? passenger = AssignToken<Passenger>(id, role);
            if(passenger != null)
            {
                return passenger;
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
            newPassenger.QrString = Guid.NewGuid();
            newPassenger.Verified = true;
            int id = _passengerLogic.AddReturnId(newPassenger);

            Passenger? passenger = AssignToken<Passenger>(id, role);
            if (passenger != null)
            {
                return passenger;
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
           int id = _driverLogic.AddReturnId(newCompanyWorker);

            CompanyWorker? companyWorker = AssignToken<CompanyWorker>(id, role);
            if (companyWorker != null)
            {
                return companyWorker;
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
        public string? setAddress(string? address)
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
                     _mailManager.sendMail(email, "Password Reset Request", otp,_configuration);
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
                         _mailManager.sendMail(email, "Password Reset Request", otp,_configuration);
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
        public bool CheckEmailandPhoneNumberForPassengers(string email, string phoneNumber)
        {
            Passenger? forPassengerEmail = _passengerLogic.GetPassengerWithEmail(email);
            Passenger? forPassengerPhone = _passengerLogic.GetPassengerWithPhoneNumber(phoneNumber);
            if (forPassengerEmail != null || forPassengerPhone != null)
            {
                return true;
            }
            return false;

        }

        public bool CheckEmailandPhoneNumberForCompanyWorker(string email,string phoneNumber)
        {
            CompanyWorker? forCompanyWorkerEmail = _driverLogic.GetCompanyWorkerWithEmail(email);
            CompanyWorker? forCompanyWorkerPhone = _driverLogic.GetCompanyWorkerWithPhoneNumber(phoneNumber);
            if (forCompanyWorkerEmail != null || forCompanyWorkerPhone != null)
            {
                return true;
            }
            return false;
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
        private Company CalculateRating(Company company, double rating)
        {
            int voteNumber = company.VotesNumber;
            double? companyRating = company.Rating;
            if (companyRating != null && companyRating != 0)
            {
                double totalRating = (double)(companyRating * voteNumber);
                double newRating = (totalRating + rating) / (voteNumber + 1);
                company.Rating = newRating;
                company.VotesNumber = voteNumber + 1;
            }
            else
            {
                company.Rating = rating;
                company.VotesNumber = voteNumber + 1;
            }
            return company;
        }
        public bool UpdateCompanyRating(int sessionId,double rating)
        {
            ShuttleSession? shuttleSession  = _shuttleSessionLogic.FindShuttleSessionById(sessionId);
            if (shuttleSession != null)
            {
                int companyID = shuttleSession.CompanyId;
                Company? company = _companyLogic.Find(companyID);
                if(company != null)
                {
                        company = CalculateRating(company, rating);
                        int companyId = company.Id;
                        bool isUpdated = _companyLogic.Update(companyId, company);
                        if (isUpdated)
                        {
                            return isUpdated;
                        }
                        return isUpdated;
                }
                return false;
            }
            return false;
        }
        public async Task<bool> UpdateSessionHistoryRating(SessionHistory sessionHistory,double rating)
        {          
            sessionHistory.Rate = (sessionHistory.RateCount * sessionHistory.Rate + rating) / (sessionHistory.RateCount + 1);
            sessionHistory.RateCount = sessionHistory.RateCount + 1;
            bool isSessionHistoryUpdated = await _sessionHistoryLogic.UpdateAsync(sessionHistory, sessionHistory.SessionId);
            if (isSessionHistoryUpdated)
            {
                return isSessionHistoryUpdated;
            }
            return isSessionHistoryUpdated;
        }
        public async Task<bool> UpdateDriverStaticticRating(DriversStatistic driverStatictic, double rating, int driverId)
        {
            
            driverStatictic.RatingAvg = (driverStatictic.RateCount * driverStatictic.RatingAvg + rating) / (driverStatictic.RateCount + 1);
            driverStatictic.RateCount = driverStatictic.RateCount + 1;
            bool isDriverStatisticUpdated = await _driversStatisticLogic.UpdateAsync(driverStatictic, driverId);
            if (isDriverStatisticUpdated)
            {
                return isDriverStatisticUpdated;
            }
            return isDriverStatisticUpdated;
        }




        public async Task<bool> UploadPhoto(IFormFile file, string name)
        {
            string fileName = name;
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                fileName = fileName + extension;
                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                   fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }           
        }

        public ShuttleManager GetPassengersLocation(int sessionId)
        {
            List<PassengerRouteDto> passengerRouteDtoList = _joinTableLogic.ShuttleManagerJoinTables(sessionId);
            ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(sessionId);
            if (shuttleSession == null)
            {
                return null;
            }
            ShuttleRouteDto shuttleRouteDto = new ShuttleRouteDto()
            {
                Id = sessionId,
                StartTime = _shuttleSessionLogic.FindShuttleSessionById(sessionId).StartTime,
                StartGeopoint = _geoPointLogic.Find((int)shuttleSession.StartGeopoint),
                FinalGeopoint = _geoPointLogic.Find((int)shuttleSession.FinalGeopoint),

            };
            ShuttleManager shuttleManager = new ShuttleManager()
            {
                PassengerRouteDto = passengerRouteDtoList,
                ShuttleRouteDto = shuttleRouteDto,
            };
            return shuttleManager;
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
