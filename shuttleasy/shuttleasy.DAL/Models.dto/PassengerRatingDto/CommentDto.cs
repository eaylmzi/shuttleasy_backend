﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.PassengerRatingDto
{
    public class CommentDto
    {
        public double Rating { get; set; }
        public int SessionId { get; set; }
        public string Comment { get; set; } = null!;
    }
}
