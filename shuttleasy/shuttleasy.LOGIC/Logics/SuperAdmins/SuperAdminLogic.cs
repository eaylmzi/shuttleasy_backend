using shuttleasy.DAL.EFRepositories.ShuttleSessions;
using shuttleasy.DAL.EFRepositories.SuperAdmins;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.SuperAdmins
{
    public class SuperAdminLogic : ISuperAdminLogic
    {
        private ISuperAdminRepository _superAdminRepository;
        public SuperAdminLogic(ISuperAdminRepository superAdminRepository)
        {
            _superAdminRepository = superAdminRepository;
        }
        public bool Add(CompanyWorker companyWorker)
        {
            bool isAdded = _superAdminRepository.Add(companyWorker);
            return isAdded;
        }

    }
}
