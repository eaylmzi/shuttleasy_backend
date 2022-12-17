using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace shuttleasy.DAL.Interfaces
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public List<T>? Get()
        {
            using (var context = new ShuttleasyDBContext())
            {
                var entity = context.Set<T>().ToList();
                return entity;
            }
            
        }
        public List<T>? Get(Func<T, bool> metot)
        {
            using (var context = new ShuttleasyDBContext())
            {
                var list = context.Set<T>()
                      .Where(metot)
                      .Select(m => m)
                      .ToList();

                return list;


            }
            
        }
        

        public bool Update(T updatedEntity, Func<T, bool> metot)
        {
            using (var context = new ShuttleasyDBContext())
            {
                T? entity = context.Set<T>()
                      .Where(metot)
                      .Select(m => m)
                      .SingleOrDefault();

                if (entity != null)
                {
                    entity = updatedEntity;
                    context.SaveChangesAsync();
                    return true;
                }

                return false;


            }
            

        }

        public bool Delete(Func<T, bool> metot)
        {
            using (var context = new ShuttleasyDBContext())
            {
                T? entity = context.Set<T>()
                     .Where(metot)
                     .Select(m => m)
                     .SingleOrDefault();
                if (entity != null)
                {
                    context.Set<T>().Remove(entity);
                    context.SaveChanges();
                    return true;

                }
                return false;

            }
            
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

        public T? GetSingle(Func<T, bool> metot)
        {
            using (var context = new ShuttleasyDBContext())
            {
                T? entity = context.Set<T>()
                                 .Where(metot)
                                 .Select(m => m)
                                 .SingleOrDefault();


                return entity;


            }
            
        }
    }
}
