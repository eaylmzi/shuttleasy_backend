﻿using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics
{
    public interface IPassengerLogic
    {
        public bool Add(Passenger passenger);
        public List<Passenger>? GetAllPassengers();
        public Passenger? GetPassengerWithEmail(string email);
        public Passenger? GetPassengerWithId(int id);
        public bool UpdatePassengerWithEmail(Passenger updatedPassenger, string email);
        public Passenger? GetPassengerWithToken(string token);
        public bool DeletePassenger(string email);
        public Passenger? GetPassengerWithPhoneNumber(string phone);
    }
}
