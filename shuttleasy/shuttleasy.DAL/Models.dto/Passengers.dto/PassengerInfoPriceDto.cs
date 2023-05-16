using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.Passengers.dto
{
    public class PassengerInfoPriceDto
    {
        public int Id { get; set; }
        public byte[]? ProfilePic { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public double Price { get; set; }
    }
}
