using shuttleasy.DAL.EFRepositories.DriversStatistics;
using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.DriversStatistics
{
    public class DriversStatisticLogic : IDriversStatisticLogic
    {
        private IDriversStatisticRepository _driversStatisticRepository;
        public DriversStatisticLogic(IDriversStatisticRepository driversStatisticRepository)
        {
            _driversStatisticRepository = driversStatisticRepository;
        }
        public bool Add(DriversStatistic driversStatistic)
        {
            bool isAdded = _driversStatisticRepository.Add(driversStatistic);
            return isAdded;
        }
        public bool Delete(int driversStatisticNumber)
        {
            Func<DriversStatistic, bool> getDriversStatistic = getDriversStatistic => getDriversStatistic.Id == driversStatisticNumber;
            bool isDeleted = _driversStatisticRepository.Delete(getDriversStatistic);
            return isDeleted;
        }
        public DriversStatistic? GetSingleDriverId(int driverId)
        {
            Func<DriversStatistic, bool> getDriversStatistic = getDriversStatistic => getDriversStatistic.DriverId == driverId;
            DriversStatistic? driversStatistic = _driversStatisticRepository.GetSingle(getDriversStatistic);
            return driversStatistic;
        }
        public async Task<bool> UpdateAsync(DriversStatistic updatedDriversStatistic, int driverId)
        {
            Func<DriversStatistic, bool> getSessionHistory = getSessionHistory => getSessionHistory.DriverId == driverId;
            bool isUpdated = await _driversStatisticRepository.UpdateAsync(getSessionHistory, updatedDriversStatistic);
            return isUpdated;
        }
    }
}
