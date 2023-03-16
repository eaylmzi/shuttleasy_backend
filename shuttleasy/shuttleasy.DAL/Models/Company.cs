using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace shuttleasy.DAL.Models
{
    public partial class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public double? Rating { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int VotesNumber { get; set; }
    }
}
