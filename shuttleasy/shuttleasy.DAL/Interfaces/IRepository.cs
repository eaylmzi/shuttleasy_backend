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
        public T? GetSingle(Func<T, bool> metot);
        public List<T>? Get();
        public bool Update(T updatedEntity, Func<T, bool> metot);
        public bool Delete(Func<T, bool> metot);

        
    }
}
