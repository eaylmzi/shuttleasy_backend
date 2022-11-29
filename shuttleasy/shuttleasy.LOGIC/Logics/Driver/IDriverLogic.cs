using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.Driver
{
    public interface IDriverLogic
    {
        public bool Add(CompanyWorker companyWorker);
        public CompanyWorker GetCompanyWorkerWithEmail(string email);
    }
}
