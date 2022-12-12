using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.SuperAdmins
{
    public interface ISuperAdminLogic
    {
        public bool Add(CompanyWorker companyWorker);
    }
}
