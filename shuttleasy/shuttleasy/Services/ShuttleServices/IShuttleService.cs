using shuttleasy.DAL.Models.dto.SessionPassengers.dto;

namespace shuttleasy.Services.ShuttleServices
{
    public interface IShuttleService
    {
        public Task<bool> EnrollPassenger(SessionPassengerDto sessionPassengerDto, int userId);
    }
}
