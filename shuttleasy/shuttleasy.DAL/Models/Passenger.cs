using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class Passenger
    {
        public string IdentityNum { get; set; } = null!;
        public byte[]? ProfilePic { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public string City { get; set; } = null!;
        public string PassengerAddress { get; set; } = null!;
        public Guid QrString { get; set; }
        public bool IsPayment { get; set; }
        public bool Verified { get; set; }
        public string Token { get; set; } = null!;
    }
}
