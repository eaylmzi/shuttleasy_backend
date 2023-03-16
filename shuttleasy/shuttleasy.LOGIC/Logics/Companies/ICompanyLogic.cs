using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.Companies
{
    public interface ICompanyLogic
    {
        public bool Add(Company company);
        public bool Delete(int companyNumber);
        public Company? Find(int companyNumber);
        public Task<Company?> FindAsync(int companyNumber);
        public bool Update(int companyId, Company updatedCompany);
        public Task<bool> UpdateAsync(int id, Company updatedCompany);
        public string? GetCompanyNameWithCompanyId(int companyId);
       
    }
}
