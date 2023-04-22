using AutoMapper;
using shuttleasy.DAL.EFRepositories.CompanyWorkers;
using shuttleasy.DAL.EFRepositories.PasswordReset;
using shuttleasy.DAL.EFRepositories;
using shuttleasy.Encryption;
using shuttleasy.JwtToken;
using shuttleasy.LOGIC.Logics.Companies;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.PasswordReset;
using shuttleasy.LOGIC.Logics.ShuttleSessions;
using shuttleasy.LOGIC.Logics;
using shuttleasy.Mail;
using shuttleasy.DAL.Models;
using shuttleasy.LOGIC.Logics.SessionPassengers;
using shuttleasy.LOGIC.Logics.PickupPoints;
using System.Collections.Generic;
using shuttleasy.DAL.Models.dto.SessionPassengers.dto;
using Org.BouncyCastle.Asn1.Ocsp;
using shuttleasy.Resource;
using shuttleasy.LOGIC.Logics.GeoPoints;
using shuttleasy.DAL.Models.dto.User.dto;

namespace shuttleasy.Services.ShuttleServices
{
    public class ShuttleService : IShuttleService
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
        private readonly ISessionPassengerLogic _sessionPassengerLogic;
        private readonly IPickupPointLogic _pickupPointLogic;
        private readonly IGeoPointLogic _geoPointLogic;
        private static IWebHostEnvironment _webHostEnvironment;




        public ShuttleService(IPassengerLogic passengerLogic, IPasswordEncryption passwordEncryption, IMapper mapper,
            IJwtTokenManager jwtTokenManager, IConfiguration configuration, ICompanyWorkerLogic driverLogic,
            IMailManager mailManager, IPasswordResetLogic passwordResetLogic, IPasswordResetRepository passwordResetRepository,
            ICompanyWorkerRepository driverRepository, IPassengerRepository passengerRepository, ICompanyLogic companyLogic,
            IShuttleSessionLogic shuttleSessionLogic, ICompanyWorkerLogic companyWorkerLogic, IWebHostEnvironment webHostEnvironment,
            ISessionPassengerLogic sessionPassengerLogic, IPickupPointLogic pickupPointLogic, IGeoPointLogic geoPointLogic)
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
            _sessionPassengerLogic = sessionPassengerLogic;
            _pickupPointLogic = pickupPointLogic;
            _geoPointLogic = geoPointLogic;
        }

        private bool IsAlreadyRegistered(int sessionId,int passengerId)
        {
            List<SessionPassenger>? sessionPassengerList = _sessionPassengerLogic.GetListById(sessionId);
            if (sessionPassengerList == null)
            {
                return false;
            }
            
            foreach (SessionPassenger list in sessionPassengerList)
            {
                int? pickupId = list.PickupId;     
                if (pickupId != null)
                {
                    int nonNullPickupId = (int)pickupId;
                    PickupPoint? pickupPoint = _pickupPointLogic.GetSingle(nonNullPickupId);
                    if (pickupPoint != null)
                    {
                        if (pickupPoint.UserId == passengerId)
                        {
                            return true;
                        }
                    }
                }             
            }
            return false;
        }
        private SessionPassenger CreateSessionPassengerWithPickupPoint(SessionPassengerDto sessionPassengerDto, int userId)
        {
            SessionPassenger sessionPassenger = _mapper.Map<SessionPassenger>(sessionPassengerDto);
            PickupPoint pickupPoint = new PickupPoint();
            int? geoPointId = _geoPointLogic.FindByCoordinate(sessionPassengerDto.Longitude, sessionPassengerDto.Latitude);
            if (geoPointId != null)
            {
                pickupPoint.GeoPointId = (int)geoPointId;
                pickupPoint.UserId = userId;
                int? pickUpId = _pickupPointLogic.AddReturnId(pickupPoint);
                if (pickUpId == null)
                {
                    return new SessionPassenger();
                }
                sessionPassenger.PickupId = pickUpId;
            }
            else
            {
                GeoPoint geoPoint = new GeoPoint();
                geoPoint.Latitude = sessionPassengerDto.Latitude;
                geoPoint.Longtitude = sessionPassengerDto.Longitude;
                int? addedGeoPointId = _geoPointLogic.AddReturnId(geoPoint);
                if (addedGeoPointId == null)
                {
                    return new SessionPassenger();
                }

                pickupPoint.GeoPointId = (int)addedGeoPointId;
                pickupPoint.UserId = userId;
                int? pickUpId = _pickupPointLogic.AddReturnId(pickupPoint);
                if (pickUpId == null)
                {
                    return new SessionPassenger();
                }

                sessionPassenger.PickupId = pickUpId;
            }
            return sessionPassenger;

        }
        public async Task<bool> EnrollPassenger(SessionPassengerDto sessionPassengerDto,int userId)
        {
            bool isRegistered = IsAlreadyRegistered(sessionPassengerDto.SessionId, userId);
            if (isRegistered)
            {
                return false;
            }

            ShuttleSession? shuttleSession = _shuttleSessionLogic.FindShuttleSessionById(sessionPassengerDto.SessionId);
            if(shuttleSession == null)
            {
                return false;
            }

            shuttleSession.PassengerCount = shuttleSession.PassengerCount + 1;
            bool isUpdated = await _shuttleSessionLogic.UpdateAsync(shuttleSession.Id, shuttleSession);
            if (!isUpdated)
            {
                return false;
            }

            SessionPassenger sessionPassenger = CreateSessionPassengerWithPickupPoint(sessionPassengerDto, userId);
            if(sessionPassenger == null)
            {
                return false;
            }

            bool isAdded = _sessionPassengerLogic.Add(sessionPassenger);
            if (isAdded)
            {
                return true;

            }
            else
            {
                shuttleSession.PassengerCount = shuttleSession.PassengerCount - 1;
                bool isShuttleUpdated = await _shuttleSessionLogic.UpdateAsync(shuttleSession.Id, shuttleSession);
                if (isShuttleUpdated)
                {
                    return false;
                }
                return false;
            }
            // return değerleri değiştirilecekse diye böyle yazdım
        }




    }
}
