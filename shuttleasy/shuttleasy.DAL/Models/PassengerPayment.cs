using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class PassengerPayment
    {
        public string PassengerIdNum { get; set; } = null!;
        public int ShuttleSessionId { get; set; }
        public bool IsPaymentVerified { get; set; }
        public DateTime PaymentDate { get; set; }

        public virtual Passenger PassengerIdNumNavigation { get; set; } = null!;
        public virtual ShuttleSession ShuttleSession { get; set; } = null!;
    }
}
