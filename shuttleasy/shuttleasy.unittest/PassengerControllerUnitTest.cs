using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using shuttleasy.Controllers;
using shuttleasy.DAL.EFRepositories.CompanyWorkers;
using shuttleasy.DAL.EFRepositories.PasswordReset;
using shuttleasy.DAL.EFRepositories;
using shuttleasy.Encryption;
using shuttleasy.JwtToken;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.PasswordReset;
using shuttleasy.Mail;
using shuttleasy.Models.dto.Login.dto;
using shuttleasy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace shuttleasy.unittest
{
    public class PassengerControllerUnitTest
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




        public PassengerControllerUnitTest(IPassengerLogic passengerLogic, IPasswordEncryption passwordEncryption, IMapper mapper,
            IJwtTokenManager jwtTokenManager, IConfiguration configuration, ICompanyWorkerLogic driverLogic,
            IMailManager mailManager, IPasswordResetLogic passwordResetLogic, IPasswordResetRepository passwordResetRepository,
            ICompanyWorkerRepository driverRepository, IPassengerRepository passengerRepository)
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


        [Fact]
        public void SetAddress_ReturnsNull_WhenInputIsNull()
        {
            // Arrange
            UserService service = new UserService(_passengerLogic, _passwordEncryption, _mapper,  _jwtTokenManager, _configuration
                , _driverLogic, _mailManager, _passwordResetLogic, _passwordResetRepository
                , _driverRepository, _passengerRepository);
            string? input = null;

            // Act
            var result = service.setAddress(input);

            // Assert
            Assert.Null(result);
        }
    }
}
