using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.EFRepositories.PassengerPayments;
using shuttleasy.DAL.Models;
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

        public bool Add(PassengerPayment passengerPayment)
        {
            bool isAdded = _passengerPaymentRepository.Add(passengerPayment);
            return isAdded;
        }
        public bool Delete(int passengerPaymentNumber)
        {
            Func<PassengerPayment, bool> getPassengerPaymentNumber = getPassengerPaymentNumber => getPassengerPaymentNumber.Id == passengerPaymentNumber;
            bool isDeleted = _passengerPaymentRepository.Delete(getPassengerPaymentNumber);
            return isDeleted;
        }
        public PassengerPayment? GetSingle(int id)
        {
            Func<PassengerPayment, bool> getPassengerRating = getPassengerRating => getPassengerRating.Id == id;

            PassengerPayment? passengerRating = _passengerPaymentRepository.GetSingle(getPassengerRating);
            return passengerRating;
        }
        public async Task<bool> UpdateAsync(int id, PassengerPayment passengerPayment)
        {
            Func<PassengerPayment, bool> getPassengerPayment = getPassengerPayment => getPassengerPayment.Id == id;
            bool isUpdated = await _passengerPaymentRepository.UpdateAsync(getPassengerPayment, passengerPayment);
            return isUpdated;
        }
        public int? AddReturnId(PassengerPayment passengerPayment)
        {
            int? shuttleId = _passengerPaymentRepository.AddReturnId(passengerPayment);
            return shuttleId;
        }
    }
}
