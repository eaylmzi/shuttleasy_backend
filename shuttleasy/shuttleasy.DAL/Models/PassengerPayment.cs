using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public class PassengerPayment
    {
        public int Id { get; set; }
        public int PassengerIdentity { get; set; }
        public int ShuttleSessionId { get; set; }
        public bool IsPaymentVerified { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
