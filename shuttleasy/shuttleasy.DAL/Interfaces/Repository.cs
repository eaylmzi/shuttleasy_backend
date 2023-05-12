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
        public async Task<bool> UpdateAsync(Func<T, bool> metot,T? updatedEntity)
        {
            T? entity = query
                     .Where(metot)
                     .Select(m => m)
                     .SingleOrDefault();

            if (entity != null)
            {
                // update the properties of the entity with the values from the updated entity
                _context.Entry(entity).CurrentValues.SetValues(updatedEntity);

                // save the changes to the database
                await _context.SaveChangesAsync();

                return true;
            }

            return false;

        }
        public bool Delete(Func<T, bool> metot, Func<T, bool> metot2)
        {
            T? entity = query
                      .AsEnumerable()
                      .Where(m => metot(m) && metot2(m))
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
        public bool DeleteList(Func<T, bool> metot)
        {
            var entityList = query
                     .Where(metot)
                     .Select(m => m)
                     .ToList();
            if (entityList != null)
            {
                for (int i = 0; i < entityList.Count; i++)
                {
                    query.Remove(entityList[i]);
                    _context.SaveChanges();
                }
             
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
        public int AddReturnId(T entity)
        {
            using (var context = new ShuttleasyDBContext())
            {
                if (entity != null)
                {
                    context.Set<T>().Add(entity);
                    context.SaveChanges();

                    // Get the generated primary key value from the entity object
                    var id = context.Entry(entity).Property("Id").CurrentValue;
                    return (int)id;
                }
                else
                {
                   throw new ArgumentNullException(); // or throw an exception
                }
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
        public T? GetSingle(int number)
        {
            return query.Find(number);
        }

        public T? GetSingle(Func<T, bool> metot)
        {
            T? entity =query
                      .Where(metot)
                      .Select(m => m)
                      .SingleOrDefault();


            return entity;
        }
        public T? GetSingle(Func<T, bool> metot , Func<T, bool> metot2)
        {

            T? entity = query
                      .AsEnumerable()
                      .Where(m => metot(m) && metot2(m))
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
        public async Task<T?> GetSingleAsync(int number)
        {
            return await query.FindAsync(number);
        }
        public int? GetId(Func<T, bool> metot, Func<T, bool> metot2)
        {
            using (var context = new ShuttleasyDBContext())
            {
                // Belirtilen kriterlere göre tek bir entity seçin veya yoksa null dönün
                T? entity = context.Set<T>().AsEnumerable()
                    .SingleOrDefault(m => metot(m) && metot2(m));
                if (entity == null)
                {
                    return null;
                }
                // Seçilen entity'nin Id özelliğini döndürün
                return (int)context.Entry(entity).Property("Id").CurrentValue;
            }
        }
    }
}
