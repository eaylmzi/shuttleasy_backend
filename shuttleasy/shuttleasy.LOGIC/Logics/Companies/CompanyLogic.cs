using shuttleasy.DAL.EFRepositories;
using shuttleasy.DAL.EFRepositories.Companies;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace shuttleasy.LOGIC.Logics.Companies
{
    public class CompanyLogic : ICompanyLogic
    {
        private ICompanyRepository _companyRepository;

        public CompanyLogic(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        public bool Add(Company company)
        {
            bool isAdded = _companyRepository.Add(company);
            return isAdded;
        }
        public bool Delete(int companyNumber) 
        {
            Func<Company, bool> getCompanyNumber = getCompanyNumber => getCompanyNumber.Id == companyNumber;
            bool isDeleted = _companyRepository.Delete(getCompanyNumber);
            return isDeleted;
        }
        public Company? Find(int companyNumber)
        {
            Func<Company, bool> getCompanyNumber = getCompanyNumber => getCompanyNumber.Id == companyNumber;
            Company? isFound = _companyRepository.GetSingle(getCompanyNumber);
            return isFound;
        }
        public async Task<Company?> FindAsync(int companyNumber)
        {
            Company? isFound = await _companyRepository.GetSingleAsync(companyNumber);
            return isFound;
        }
        public bool Update(int companyId, Company updatedCompany)
        {
            Func<Company, bool> getCompanyNumber = getCompanyNumber => getCompanyNumber.Id == companyId;
            bool isUpdated =  _companyRepository.Update(updatedCompany, getCompanyNumber);
            return isUpdated;
        }
        public async Task<bool> UpdateAsync(int id,Company updatedCompany)
        {
            Func<Company, bool> getCompanyNumber = getCompanyNumber => getCompanyNumber.Id == id;
            bool isUpdated =await _companyRepository.UpdateAsync(getCompanyNumber, updatedCompany);
            return isUpdated;
        }

        public string? GetCompanyNameWithCompanyId(int companyId)
        {
            Func<Company, bool> getCompany = getCompany => getCompany.Id == companyId;
            Company? company = _companyRepository.GetSingle(getCompany);
            return company.Name;
        }
        
    }
}
