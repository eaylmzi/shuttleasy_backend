using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.EFRepositories.PassengerPayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.PassengerPayments
{
    public class PassengerPaymentLogic : IPassengerPaymentLogic
    {
        private IPassengerPaymentRepository _passengerPaymentRepository;
        public PassengerPaymentLogic(IPassengerPaymentRepository passengerPaymentRepository)
        {
            _passengerPaymentRepository = passengerPaymentRepository;
        }
    }
}
