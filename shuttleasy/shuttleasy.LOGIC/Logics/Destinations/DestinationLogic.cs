using shuttleasy.DAL.EFRepositories.CompanyWorkers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.Destinations
{
    public class DestinationLogic : IDestinationLogic
    {

        private IDestinationLogic _destinationLogic;
        public DestinationLogic(IDestinationLogic destinationLogic)
        {
            _destinationLogic = destinationLogic;
        }
    }
}
