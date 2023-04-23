using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.DriversStatistics
{
    public interface IDriversStatisticLogic
    {
        public bool Add(DriversStatistic driversStatistic);
        public bool Delete(int driversStatisticNumber);
        public DriversStatistic? GetSingleDriverId(int driverId);
        public Task<bool> UpdateAsync(DriversStatistic updatedDriversStatistic, int driverId);
    }
}
