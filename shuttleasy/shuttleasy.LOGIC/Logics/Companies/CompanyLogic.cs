using shuttleasy.DAL.EFRepositories;
using shuttleasy.DAL.EFRepositories.Companies;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.Companies
{
    public class CompanyLogic : ICompanyLogic
    {
        private ICompanyRepository _companyRepository;

        public CompanyLogic(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public string? GetCompanyNameWithCompanyId(int companyId)
        {
            Func<Company, bool> getCompany = getCompany => getCompany.Id == companyId;
            Company? company = _companyRepository.GetSingle(getCompany);
            return company.Name;
        }
        
    }
}
