using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.CompanyWorkers
{
    public class CompanyWorkerRepository : Repository<CompanyWorker>, ICompanyWorkerRepository
    {
        ShuttleasyDBContext _context = new ShuttleasyDBContext();

        private DbSet<CompanyWorker> companyWorkerTable { get; set; }
        public CompanyWorkerRepository()
        {
            companyWorkerTable = _context.Set<CompanyWorker>();
        }
        public async Task<bool> IsPhoneNumberAndEmailExist(string email, string phoneNumber)
        {
            return await companyWorkerTable.AnyAsync(entity => entity.Email == email) &&
                await companyWorkerTable.AnyAsync(entity => entity.PhoneNumber == phoneNumber);
        }
    }
}
