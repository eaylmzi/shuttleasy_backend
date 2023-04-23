using shuttleasy.DAL.EFRepositories.PassengerPayments;
using shuttleasy.DAL.EFRepositories.PassengerRatings;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.PassengerRatings
{
    public class PassengerRatingLogic : IPassengerRatingLogic
    {
        private IPassengerRatingRepository _passengerRatingRepository;
        public PassengerRatingLogic(IPassengerRatingRepository passengerRatingRepository)
        {
            _passengerRatingRepository = passengerRatingRepository;
        }

        public bool Add(PassengerRating passengerRating)
        {
            bool isAdded = _passengerRatingRepository.Add(passengerRating);
            return isAdded;
        }
        public bool Delete(int passengerRatingNumber) 
        {
            Func<PassengerRating, bool> getPassengerRatingNumber = getPassengerRatingNumber => getPassengerRatingNumber.Id == passengerRatingNumber;
            bool isDeleted = _passengerRatingRepository.Delete(getPassengerRatingNumber);
            return isDeleted;
        }
        public PassengerRating? GetSingle(int passengerId, int shuttleId)
        {
            Func<PassengerRating, bool> getPassengerRatingNumberByPassengerId = getPassengerRatingNumberByPassengerId => getPassengerRatingNumberByPassengerId.PassengerIdentity == passengerId;
            Func<PassengerRating, bool> getPassengerRatingNumberByShuttleId = getPassengerRatingNumberByShuttleId => getPassengerRatingNumberByShuttleId.SessionId == shuttleId;
            PassengerRating? passengerRating = _passengerRatingRepository.GetSingle(getPassengerRatingNumberByPassengerId, getPassengerRatingNumberByShuttleId);
            return passengerRating;
        }

    }
}
