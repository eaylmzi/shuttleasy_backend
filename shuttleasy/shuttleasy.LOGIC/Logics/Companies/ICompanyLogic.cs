using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.Companies
{
    public interface ICompanyLogic
    {
        public string? GetCompanyNameWithCompanyId(int companyId);
    }
}
