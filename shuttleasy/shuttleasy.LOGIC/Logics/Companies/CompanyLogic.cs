using shuttleasy.DAL.EFRepositories;
using shuttleasy.DAL.EFRepositories.Companies;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
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

        public string? GetCompanyNameWithCompanyId(int companyId)
        {
            Func<Company, bool> getCompany = getCompany => getCompany.Id == companyId;
            Company? company = _companyRepository.GetSingle(getCompany);
            return company.Name;
        }
        
    }
}
