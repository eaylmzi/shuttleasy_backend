using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.PassengerPayments
{
    public interface IPassengerPaymentLogic
    {
        public bool Add(PassengerPayment passengerPayment);
        public bool Delete(int passengerPaymentNumber);
        public PassengerPayment? GetSingle(int id);
        public  Task<bool> UpdateAsync(int id, PassengerPayment passengerPayment);
        public int? AddReturnId(PassengerPayment passengerPayment);
    
    }
}
