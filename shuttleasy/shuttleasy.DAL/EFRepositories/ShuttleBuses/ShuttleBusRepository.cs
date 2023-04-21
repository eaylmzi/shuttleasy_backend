using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.ShuttleBuses.dto;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.ShuttleBuses
{
    public class ShuttleBusRepository : Repository<ShuttleBus>, IShuttleBusRepository
    {
        ShuttleasyDBContext _context = new ShuttleasyDBContext();
        public async Task<bool> CheckAllForeignKeysAndUniqueExistAsync(ShuttleBusDto shuttleBusDto)
        {
            bool licensePlateExists = await _context.Set<ShuttleBus>().AnyAsync(c => c.LicensePlate == shuttleBusDto.LicensePlate);
            bool companyIdExists = await _context.Set<Company>().AnyAsync(c => c.Id == shuttleBusDto.CompanyId);

            return companyIdExists && !licensePlateExists;
        }
    }
}
