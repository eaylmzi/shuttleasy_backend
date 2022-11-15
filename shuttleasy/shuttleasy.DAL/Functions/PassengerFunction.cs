using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;

namespace shuttleasy.DAL.Functions
{
    public class PassengerFunction : Repository<Passenger>, IPassengerRepository
    {
        public Passenger GetPassengerWithEmail(string email)
        {
            Func<Passenger, bool> get = pas => pas.Email == email;
            return GetSingle(get);
            //Birden fazla kullanıcı döndürebilir, exception yazılabilir  
        }

        public Passenger GetPassenger(string id)
        {
            try 
            {
                var pas = Get(id);
                    
                
                foreach (var passenger in pas)
                {
                    if (passenger.IdentityNum == id)
                    {
                        return passenger;
                    }
                }
                return new Passenger();
               
            }
            catch(Exception ex)

            {
                object o = ex.Message;
                return (Passenger)o;

            }
         //El atacam   
        }
        

        public bool isPaid(string id)
        {
          
            throw new NotImplementedException();
        }

        public bool AddPassenger(Passenger passenger)
        {
            if (CheckNullability(passenger))
            {
                bool isAdded = Add(passenger);
                return isAdded;
            }
            return false;
            
        }

        private bool CheckNullability(Passenger passenger)
        {
            if (string.IsNullOrEmpty(passenger.IdentityNum))
            {
                throw new ArgumentNullException(nameof(passenger.IdentityNum));
            }
            else if (passenger.IdentityNum.Length != 11)
            {
                // TC kimliği inte çevirip sayımı diye kontrol edebiliriz
                throw new ArgumentException("The lenght of identity number is not valid");
            }
            else if (string.IsNullOrEmpty(passenger.Name))
            {
                throw new ArgumentNullException(nameof(passenger.Name));
            }
            else if (string.IsNullOrEmpty(passenger.Surname))
            {
                throw new ArgumentNullException(nameof(passenger.Surname));
            }
            else if (string.IsNullOrEmpty(passenger.PhoneNumber))
            {
                throw new ArgumentNullException(nameof(passenger.PhoneNumber));
            }
            else if (passenger.PhoneNumber.Length != 10)
            {
                throw new ArgumentNullException("The lenght of phone number is not valid");
            }
            else if (string.IsNullOrEmpty(passenger.Email))
            {
                throw new ArgumentNullException(nameof(passenger.Email));
            }
            else if ((passenger.PasswordHash == null || passenger.PasswordHash.Length == 0) && (passenger.PasswordSalt == null || passenger.PasswordSalt.Length == 0))
            {
                throw new ArgumentNullException(nameof(passenger.PasswordHash) + nameof(passenger.PasswordSalt));
            }
            else if (string.IsNullOrEmpty(passenger.City))
            {
                throw new ArgumentNullException(nameof(passenger.City));
            }
            else if (string.IsNullOrEmpty(passenger.PassengerAddress))
            {
                throw new ArgumentNullException(nameof(passenger.PassengerAddress));
            }

            return true;
        }
    }
}
