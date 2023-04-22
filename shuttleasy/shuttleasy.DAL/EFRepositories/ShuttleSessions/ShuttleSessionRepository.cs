using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.EFRepositories.ShuttleBuses;
using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.ShuttleBuses.dto;
using shuttleasy.DAL.Models.dto.ShuttleSessions.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.ShuttleSessions
{
    public class ShuttleSessionRepository : Repository<ShuttleSession>, IShuttleSessionRepository
    {
        ShuttleasyDBContext _context = new ShuttleasyDBContext();
        public async Task<bool> CheckAllForeignKeysAndUniqueExistAsync(ShuttleSessionDto shuttleSessionDto)
        {
            bool companyIdExists = await _context.Set<Company>().AnyAsync(c => c.Id == shuttleSessionDto.CompanyId);
            bool busIdExists = await _context.Set<ShuttleBus>().AnyAsync(c => c.Id == shuttleSessionDto.BusId);
            bool driverIdExists = await _context.Set<CompanyWorker>().AnyAsync(c => c.Id == shuttleSessionDto.DriverId);
            bool startGeoPointExists = await _context.Set<GeoPoint>().AnyAsync(c => c.Id == shuttleSessionDto.StartGeopoint);
            bool finalGeoPointExists = await _context.Set<GeoPoint>().AnyAsync(c => c.Id == shuttleSessionDto.FinalGeopoint);

            return companyIdExists && busIdExists && driverIdExists && startGeoPointExists && finalGeoPointExists;
        }
    }
}
