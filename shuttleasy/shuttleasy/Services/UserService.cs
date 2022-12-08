﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
using System;

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
        public Passenger SignUp(PassengerRegisterDto passengerRegisterDto,string role)
        {
            Passenger newPassenger = new Passenger();
            newPassenger = _mapper.Map<Passenger>(passengerRegisterDto);
            newPassenger.QrString = Guid.NewGuid();
            newPassenger.IsPayment = false;

            string token = _jwtTokenManager.CreateToken(newPassenger, role, _configuration);
            newPassenger.Token = token;
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
            return newPassenger;

        }
        public Passenger CreatePassenger(PassengerRegisterPanelDto passengerRegisterPanelDto, string role)
        {
            Passenger newPassenger = new Passenger();
            newPassenger = _mapper.Map<Passenger>(passengerRegisterPanelDto);
            string randomPassword = GetRandomString(10);
            _passwordEncryption.CreatePasswordHash(randomPassword, out byte[] passwordHash, out byte[] passwordSalt);
            newPassenger.PasswordHash = passwordHash;
            newPassenger.PasswordSalt = passwordSalt;

            newPassenger.Verified = true;

            string token = _jwtTokenManager.CreateToken(newPassenger, role, _configuration);
            newPassenger.Token = token;

            _passengerLogic.Add(newPassenger);
            return newPassenger;

        }
        public CompanyWorker CreateDriver(DriverRegisterDto driverRegisterDto, string role)
        {
            CompanyWorker newCompanyWorker = new CompanyWorker();
            newCompanyWorker = _mapper.Map<CompanyWorker>(driverRegisterDto);
            string randomPassword = GetRandomString(10);
            _passwordEncryption.CreatePasswordHash(randomPassword, out byte[] passwordHash, out byte[] passwordSalt);
            newCompanyWorker.PasswordHash = passwordHash;
            newCompanyWorker.PasswordSalt = passwordSalt;
            
            newCompanyWorker.Verified = true;
            newCompanyWorker.WorkerType = role;

            string token = _jwtTokenManager.CreateToken(newCompanyWorker, role, _configuration);
            newCompanyWorker.Token = token;

            _driverLogic.Add(newCompanyWorker);
            return newCompanyWorker;

        }


        public Passenger UpdateProfile(Passenger passenger)
        {
            return new Passenger();
        }
        public CompanyWorker UpdateDriver(CompanyWorker companyWorker)
        {
            return new CompanyWorker();
        }






        public ResetPassword? SendOTP(string email)
        {
            if (!isAnyValidOTP(email))
            {
                string otp = GetRandomOTP(6);
                // _mailManager.sendMail(email, "Password Reset Request", otp,_configuration);
                ResetPassword resetPassword = new ResetPassword();
                resetPassword.Email = email;
                resetPassword.Date = DateTime.Now;
                resetPassword.ResetKey = otp;
                _passwordResetLogic.Add(resetPassword);
                return resetPassword;

            }
            return null;
        }
        public EmailTokenDto? ValidateOTP(string email,string otp)
        {
            ResetPassword resetPassword = _passwordResetLogic.GetResetPasswordWithEmail(email)
                 ?? throw new ArgumentNullException();
            Passenger? passenger;
            CompanyWorker companyWorker;

            if (resetPassword != null)
            {
                if (resetPassword.ResetKey.Equals(otp))
                {

                    int time = getTimeDiffereance(resetPassword.Date);
                    if (time < 180)
                    {
                        EmailTokenDto emailTokenDto;
                        passenger = _passengerLogic.GetPassengerWithEmail(email);
                        if (passenger!= null)//Geliştirilebilir bak buraya
                        {
                            emailTokenDto = new EmailTokenDto();
                            emailTokenDto.Email = email;
                            emailTokenDto.Token = passenger.Token;
                        }
                        else
                        {
                            companyWorker = _driverLogic.GetCompanyWorkerWithEmail(email)
                                    ?? throw new ArgumentNullException();
                            emailTokenDto = new EmailTokenDto();
                            emailTokenDto.Email = email;
                            emailTokenDto.Token = companyWorker.Token;
                        }


                        return emailTokenDto;
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
                    return passenger;
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
                bool isUpdated = _driverLogic.UpdateDriverWithEmail(companyWorker, email);
                if (isUpdated)
                {
                    return companyWorker;
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
