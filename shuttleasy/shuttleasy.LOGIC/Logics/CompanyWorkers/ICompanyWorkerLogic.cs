using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.CompanyWorkers
{
    public interface ICompanyWorkerLogic
    {
        public bool Add(CompanyWorker companyWorker);
        public int AddReturnId(CompanyWorker companyWorker);
        public CompanyWorker? GetSingle(int id);
        public CompanyWorker? GetCompanyWorkerWithId(int id);
        public CompanyWorker? GetCompanyWorkerWithEmail(string email);
        public CompanyWorker? GetCompanyWorkerWithToken(string token);
        public bool UpdateCompanyWorkerWithEmail(CompanyWorker updatedCompanyWorker, string email);
        public List<CompanyWorker>? GetAllDriverWithCompanyId(int companyId);
        public CompanyWorker? GetCompanyWorkerWithPhoneNumber(string phone);
        public Task<bool> UpdateAsync(int id, CompanyWorker updatedCompanyWorker);
        public Task<bool> IsPhoneNumberAndEmailExist(string email, string phoneNumber);
    }
}
