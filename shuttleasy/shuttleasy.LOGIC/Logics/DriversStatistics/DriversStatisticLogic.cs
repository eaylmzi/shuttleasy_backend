using shuttleasy.DAL.EFRepositories.DriversStatistics;
using shuttleasy.DAL.EFRepositories.GeoPoints;
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
    }
}
