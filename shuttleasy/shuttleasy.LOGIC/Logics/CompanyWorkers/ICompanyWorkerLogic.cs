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
        public CompanyWorker? GetCompanyWorkerWithId(int id);
        public CompanyWorker? GetCompanyWorkerWithEmail(string email);
        public CompanyWorker? GetCompanyWorkerWithToken(string token);
        public bool UpdateCompanyWorkerWithEmail(CompanyWorker updatedCompanyWorker, string email);
        public List<CompanyWorker>? GetAllDriverWithCompanyId(int companyId);
        public CompanyWorker? GetCompanyWorkerWithPhoneNumber(string phone);
    }
}
