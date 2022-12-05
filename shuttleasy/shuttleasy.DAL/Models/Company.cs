using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public double? Rating { get; set; }

    }
}
