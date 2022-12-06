using shuttleasy.DAL.EFRepositories.CompanyWorkers;
using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.PasswordReset
{
    public class PasswordResetRepository : Repository<ResetPassword>, IPasswordResetRepository //sadece interfaceyi eklesek oluyo mu
    {

    }
}
