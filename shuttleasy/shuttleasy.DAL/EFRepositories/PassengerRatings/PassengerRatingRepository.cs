﻿using shuttleasy.DAL.EFRepositories.NotificationWorkers;
using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.PassengerRatings
{
    public class PassengerRatingRepository : Repository<PassengerRating>, IPassengerRatingRepository
    {
    }
}
