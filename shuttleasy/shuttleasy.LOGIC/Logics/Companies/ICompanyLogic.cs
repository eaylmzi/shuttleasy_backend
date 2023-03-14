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
        public string? GetCompanyNameWithCompanyId(int companyId);
    }
}
