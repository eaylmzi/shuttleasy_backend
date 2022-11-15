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
            _passenger.AddPassenger(passenger);
            return true;
        }

        public Passenger GetPassengerWithEmail(string email)
        {
            return _passenger.GetPassengerWithEmail(email);
        }

        public Passenger Get(string id)
        {
            if (id.Length == 11)
            {
                return _passenger.GetPassenger(id);
            }
            return new Passenger();


        }
    }
}
