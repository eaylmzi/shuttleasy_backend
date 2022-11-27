using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shuttleasy.DAL.EFRepositories;
using shuttleasy.DAL.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace shuttleasy.LOGIC.Logics
{
    public class PassengerLogic : IPassengerLogic
    {
        private IPassengerRepository _passenger;

        public PassengerLogic(IPassengerRepository passenger)
        {
            _passenger = passenger;
        }

        public bool Add(Passenger passenger)
        {
            bool isAdded = _passenger.Add(passenger);
            return isAdded;
        }
        public List<Passenger> GetAllPassengers()// yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            var passengerList = _passenger.Get() ?? throw new ArgumentNullException();
            return passengerList;
        }

        public Passenger GetPassengerWithEmail(string email) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<Passenger, bool> getPassenger = pas => pas.Email == email;
            try
            {
                Passenger passenger = _passenger.GetSingle(getPassenger) ?? throw new ArgumentNullException();
                return passenger;
            }
            catch(Exception)
            {
                return new Passenger();
            }
            
            
        }

        public Passenger GetPassengerWithId(string id) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<Passenger, bool> getPassenger = pas => pas.IdentityNum == id;
            Passenger passenger = _passenger.GetSingle(getPassenger) ?? throw new ArgumentNullException();
            return passenger;
        }
    }
}
