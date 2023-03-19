using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {

        public bool Add(T entity);
        public int AddReturnId(T entity);
        public Task<bool> AddAsync(T entity);
        public T? GetSingle(Func<T, bool> metot);
        public T? GetSingle(Func<T, bool> metot, Func<T, bool> metot2);
        // public Task<T?> GetSingleAsync(Func<T, bool> metot);
        public Task<T?> GetSingleAsync(int number);
        public List<T>? Get();
        public List<T>? Get(Func<T, bool> metot);
        public bool Update(T updatedEntity, Func<T, bool> metot);
        public Task<bool> UpdateAsync(Func<T, bool> metot, T? updatedEntity);
        public bool Delete(Func<T, bool> metot);

        
    }
}
