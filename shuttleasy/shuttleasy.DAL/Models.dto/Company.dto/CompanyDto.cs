using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.Company.dto
{
    public class CompanyDto
    {
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public double? Rating { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
