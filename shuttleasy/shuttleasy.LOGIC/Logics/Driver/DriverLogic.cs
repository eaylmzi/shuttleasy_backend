using shuttleasy.DAL.EFRepositories;
using shuttleasy.DAL.EFRepositories.Driver;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.Driver
{
    public class DriverLogic : IDriverLogic
    {
        private IDriverRepository _driverRepository;
        public DriverLogic(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        public bool Add(CompanyWorker companyWorker)
        {
            bool isAdded = _driverRepository.Add(companyWorker);
            return isAdded;

        }
        public CompanyWorker GetCompanyWorkerWithEmail(string email) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<CompanyWorker, bool> getCompanyWorker = getCompanyWorker => getCompanyWorker.Email == email;
            try
            {
                CompanyWorker companyWorker = _driverRepository.GetSingle(getCompanyWorker) ?? throw new ArgumentNullException();
                return companyWorker;
            }
            catch (Exception)
            {
                return new CompanyWorker();
            }


        }
    }
}
