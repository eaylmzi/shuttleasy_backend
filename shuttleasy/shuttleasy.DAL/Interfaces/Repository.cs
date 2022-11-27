using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace shuttleasy.DAL.Interfaces
{
    public class Repository<T> : IRepository<T> where T : class
    {
        ShuttleasyDBContext _context = new ShuttleasyDBContext();

        private DbSet<T> query { get; set; }
        public Repository() {
            query = _context.Set<T>();
        }

        public List<T>? Get()
        {
            var entity = query.ToList();
            return entity;
        }

        public bool Update(T entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public bool Add(T entity)
        {
            if (entity != null)
            {
                query.Add(entity);
                _context.SaveChangesAsync();
                return true;

            }
            else
            {
                return false;
            };
        }

        public T? GetSingle(Func<T, bool> metot)
        {
            T? entity = query
                      .Where(metot)
                      .Select(m => m)
                      .SingleOrDefault();
            

            return entity;
        }
    }
}
