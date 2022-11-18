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
        public Passenger? GetPassengerWithEmail(string email)
        {
            Func<Passenger, bool> get = pas => pas.Email == email;
            return GetSingle(get);
            //Birden fazla kullanıcı döndürebilir, exception yazılabilir  
        }
        public Passenger? GetPassengerWithId(string id)
        {
            Func<Passenger, bool> get = pas => pas.IdentityNum == id;
            return GetSingle(get);
            //Birden fazla kullanıcı döndürebilir, exception yazılabilir  
        }




        public bool isPaid(string id)
        {         
            throw new NotImplementedException();
        }
   
    }
}
