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
        public CompanyWorker? GetCompanyWorkerWithEmail(string email);
        public bool UpdateDriverWithEmail(CompanyWorker updatedCompanyWorker, string email);
    }
}
