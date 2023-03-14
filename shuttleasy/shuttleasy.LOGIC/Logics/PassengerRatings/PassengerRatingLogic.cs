using shuttleasy.DAL.EFRepositories.PassengerPayments;
using shuttleasy.DAL.EFRepositories.PassengerRatings;
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
    }
}
