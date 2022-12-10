using shuttleasy.DAL.EFRepositories.CompanyWorkers;
using shuttleasy.DAL.EFRepositories.Destinations;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.Destinations
{
    public class DestinationLogic : IDestinationLogic
    {

        private IDestinationRepository _destinationRepository;
        public DestinationLogic(IDestinationRepository destinationRepository)
        {
            _destinationRepository = destinationRepository;
        }


        public bool Add(Destination destination)
        {
            bool isAdded = _destinationRepository.Add(destination);
            return isAdded;
        }
        public bool DeleteDestination(int destinationNumber) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<Destination, bool> getDestinationNumber = getDestinationNumber => getDestinationNumber.Id == destinationNumber;
            bool isDeleted = _destinationRepository.Delete(getDestinationNumber);
            return isDeleted;
        }
    }
}
