using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.CompanyWorkers
{
    public interface ICompanyWorkerRepository : IRepository<CompanyWorker>
    {
        public Task<bool> IsPhoneNumberAndEmailExist(string email, string phoneNumber);
    }
}
