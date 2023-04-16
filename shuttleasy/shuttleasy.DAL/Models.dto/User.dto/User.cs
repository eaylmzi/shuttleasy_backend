using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.User.dto
{
    public class User<T>
    {
        public T? UserEntity { get; set; }
    }
}
