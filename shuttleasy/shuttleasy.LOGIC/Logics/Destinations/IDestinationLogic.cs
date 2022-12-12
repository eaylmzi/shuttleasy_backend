using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.Destinations
{
    public interface IDestinationLogic
    {
        public bool Add(Destination destination);
        public bool DeleteDestination(int destinationNumber);
        public Destination? FindDestinationWithBeginningDestination(string beginningDestination);
    }
}
