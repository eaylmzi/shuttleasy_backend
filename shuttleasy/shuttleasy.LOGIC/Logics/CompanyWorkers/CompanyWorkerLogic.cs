using shuttleasy.DAL.EFRepositories;
using shuttleasy.DAL.EFRepositories.CompanyWorkers;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.CompanyWorkers
{
    public class CompanyWorkerLogic : ICompanyWorkerLogic
    {
        private ICompanyWorkerRepository _companyWorkerRepository;
        public CompanyWorkerLogic(ICompanyWorkerRepository companyWorkerRepository)
        {
            _companyWorkerRepository = companyWorkerRepository;
        }

        public bool Add(CompanyWorker companyWorker)
        {
            bool isAdded = _companyWorkerRepository.Add(companyWorker);
            return isAdded;

        }
        public int AddReturnId(CompanyWorker companyWorker)
        {
            int id = _companyWorkerRepository.AddReturnId(companyWorker);
            return id;
        }
        public CompanyWorker? GetSingle(int id)
        {
            CompanyWorker? companyWorker = _companyWorkerRepository.GetSingle(id);
            return companyWorker;
        }
        public CompanyWorker? GetCompanyWorkerWithEmail(string email) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<CompanyWorker, bool> getCompanyWorker = getCompanyWorker => getCompanyWorker.Email == email;
            CompanyWorker? companyWorker = _companyWorkerRepository.GetSingle(getCompanyWorker);
            return companyWorker;

        }
        public CompanyWorker GetCompanyWorkerWithId(int id)
        {
            Func<CompanyWorker, bool> getDriver = getDriver => getDriver.Id == id;
            CompanyWorker companyWorker = _companyWorkerRepository.GetSingle(getDriver) ?? throw new ArgumentNullException();
            return companyWorker;
        }
        public CompanyWorker? GetCompanyWorkerWithToken(string token)
        {
            Func<CompanyWorker, bool> getCompanyWorker = getCompanyWorker => getCompanyWorker.Token == token;
            CompanyWorker? companyWorker = _companyWorkerRepository.GetSingle(getCompanyWorker);
            return companyWorker;

        }
        public CompanyWorker? GetCompanyWorkerWithPhoneNumber(string phone) 
        {
            Func<CompanyWorker, bool> getCompanyWorker = getCompanyWorker => getCompanyWorker.PhoneNumber == phone;
            CompanyWorker? companyWorker = _companyWorkerRepository.GetSingle(getCompanyWorker);
            return companyWorker;

        }

        public bool UpdateCompanyWorkerWithEmail(CompanyWorker updatedCompanyWorker, string email)
        {
            Func<CompanyWorker, bool> getDriver = pas => pas.Email == email;
            bool isUpdated = _companyWorkerRepository.Update(updatedCompanyWorker, getDriver);
            return isUpdated;        
        }

        public List<CompanyWorker>? GetAllDriverWithCompanyId(int companyId)
        {
            Func<CompanyWorker, bool> getDriver = getDriver => getDriver.CompanyId == companyId;
            List<CompanyWorker>? getDriverList = _companyWorkerRepository.Get(getDriver);
            return getDriverList;
        }

        public async Task<bool> IsPhoneNumberAndEmailExist(string email, string phoneNumber)
        {
            bool isExist = await _companyWorkerRepository.IsPhoneNumberAndEmailExist(email, phoneNumber);
            return isExist;
        }
    }
}
