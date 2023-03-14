using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.PassengerRatings
{
    public interface IPassengerRatingLogic
    {
        public bool Add(PassengerRating passengerRating);
        public bool Delete(int passengerRatingNumber);
    }
}
