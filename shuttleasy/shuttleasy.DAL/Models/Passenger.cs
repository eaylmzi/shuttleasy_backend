
using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class Passenger
    {


        public int Id { get; set; }
        public byte[]? ProfilePic { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? PassengerAddress { get; set; }
        public Guid QrString { get; set; }
        public bool IsPayment { get; set; }
        public bool Verified { get; set; }
        public string Token { get; set; } = null!;

    }
}
