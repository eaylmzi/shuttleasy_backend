using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace shuttleasy.DAL.Interfaces
{
    public class Repository<T> : IRepository<T> where T : class
    {
        ShuttleasyDBContext _context = new ShuttleasyDBContext();

        private DbSet<T> query { get; set; }
        public Repository()
        {
            query = _context.Set<T>();
        }

        public List<T>? Get()
        {
            var entity = query.ToList();
            return entity;
        }
        public List<T>? Get(Func<T, bool> metot)
        {
            var list = query
                      .Where(metot)
                      .Select(m => m)
                      .ToList();

            return list;
        }


        public bool Update(T updatedEntity, Func<T, bool> metot)
        {

            T? entity = query
                      .Where(metot)
                      .Select(m => m)
                      .SingleOrDefault();

            if (entity != null)
            {
                entity = updatedEntity;
                _context.SaveChangesAsync();
                return true;
            }

            return false;

        }

        public bool Delete(Func<T, bool> metot)
        {
            T? entity = query
                     .Where(metot)
                     .Select(m => m)
                     .SingleOrDefault();
            if (entity != null)
            {
                query.Remove(entity);
                _context.SaveChanges();
                return true;

            }
            return false;
        }

        public bool Add(T entity)
        {
            using(var context = new ShuttleasyDBContext())
            {
                if (entity != null)
                {
                    context.Set<T>().Add(entity);
                    context.SaveChanges();
                    return true;

                }
                else
                {
                    return false;
                };
            }
        }
        public async Task<bool> AddAsync(T entity)
        {
            using (var context = new ShuttleasyDBContext())
            {
                if (entity != null)
                {
                    await context.Set<T>().AddAsync(entity);
                    context.SaveChanges();
                    return true;

                }
                else
                {
                    return false;
                };
            }
        }

        public T? GetSingle(Func<T, bool> metot)
        {
            T? entity =query
                      .Where(metot)
                      .Select(m => m)
                      .SingleOrDefault();


            return entity;
        }
        /*
        public async Task<T?> GetSingleAsync(Func<T, bool> metot)
        {
            return await query.FirstOrDefaultAsync(metot);
        }
        */
    
    }
}
