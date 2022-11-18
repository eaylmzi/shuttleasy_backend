using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Functions;
using shuttleasy.DAL.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace shuttleasy.LOGIC.Logics
{
    public class PassengerLogic
    {
        private PassengerFunction _passenger = new shuttleasy.DAL.Functions.PassengerFunction();

        public bool Add(Passenger passenger)
        {
            bool isAdded = _passenger.Add(passenger);
            return isAdded;
        }

        public Passenger GetPassengerWithEmail(string email)
        {
            Passenger isFound = _passenger.GetPassengerWithEmail(email) ?? throw new ArgumentNullException();
            return isFound;      
        }

        public Passenger GetPassengerWithId(string id)
        {
            Passenger isFound = _passenger.GetPassengerWithId(id) ?? throw new ArgumentNullException();
            return isFound;
        }
    }
}
