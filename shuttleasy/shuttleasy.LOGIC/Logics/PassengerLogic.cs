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

        public int AddReturnId(Passenger passenger)
        {
            int id = _passenger.AddReturnId(passenger);
            return id;
        }
        public List<Passenger>? GetAllPassengers()
        {
            var passengerList = _passenger.Get();
            return passengerList;
        }
        public Passenger? GetSingle(int id)
        {
            Passenger? passenger = _passenger.GetSingle(id);
            return passenger;
        }

        public Passenger? GetPassengerWithEmail(string email) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<Passenger, bool> getPassenger = pas => pas.Email == email;
            Passenger? passenger = _passenger.GetSingle(getPassenger);
            return passenger;
        }

        public Passenger? GetPassengerWithId(int id) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<Passenger, bool> getPassenger = pas => pas.Id == id;
            Passenger? passenger = _passenger.GetSingle(getPassenger);
            return passenger;
        }

        public bool UpdatePassengerWithEmail(Passenger updatedPassenger,string email)
        {
            Func<Passenger, bool> getPassenger = pas => pas.Email == email;
            bool isUpdated = _passenger.Update(updatedPassenger, getPassenger);
            return isUpdated;
        }
        public Passenger? GetPassengerWithToken(string token) 
        {
            Func<Passenger, bool> getPassenger = getPassenger => getPassenger.Token == token;
            Passenger? passenger = _passenger.GetSingle(getPassenger);
            return passenger;

        }
        public Passenger? GetPassengerWithPhoneNumber(string phone)
        {
            Func<Passenger, bool> getPassenger = getPassenger => getPassenger.PhoneNumber == phone;
            Passenger? passenger = _passenger.GetSingle(getPassenger);
            return passenger;

        }
        public bool DeletePassenger(string email)
        {   
            Func<Passenger, bool> getPassenger = pas => pas.Email == email;
            bool isDeleted = _passenger.Delete(getPassenger);
            return isDeleted;
        }
    }
}
