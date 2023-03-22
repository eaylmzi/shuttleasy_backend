using shuttleasy.DAL.EFRepositories.PickupAreas;
using shuttleasy.DAL.EFRepositories.PickupPoints;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.PickupPoints
{
    public class PickupPointLogic : IPickupPointLogic
    {
        private IPickupPointRepository _pickupPointRepository;
        public PickupPointLogic(IPickupPointRepository pickupPointRepository)
        {
            _pickupPointRepository = pickupPointRepository;
        }
        public bool Add(PickupPoint pickupPoint)
        {
            bool isAdded = _pickupPointRepository.Add(pickupPoint);
            return isAdded;
        }
        public int? AddReturnId(PickupPoint pickupPoint)
        {
            int? isAdded = _pickupPointRepository.AddReturnId(pickupPoint);
            return isAdded;
        }
        public bool Delete(int pickupPointNumber)
        {
            Func<PickupPoint, bool> getPickupPointNumber = getPickupPointNumber => getPickupPointNumber.Id == pickupPointNumber;
            bool isDeleted = _pickupPointRepository.Delete(getPickupPointNumber);
            return isDeleted;
        }
    }
}
