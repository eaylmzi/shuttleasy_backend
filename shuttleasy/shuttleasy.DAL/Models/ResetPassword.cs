using System;
using System.Collections.Generic;

namespace shuttleasy.DAL.Models
{
    public partial class ResetPassword
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string ResetKey { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
