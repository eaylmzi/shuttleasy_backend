using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.EFRepositories.PickupAreas;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.PickupAreas
{
    public class PickupAreaLogic : IPickupAreaLogic
    {
        private IPickupAreaRepository _pickupAreaRepository;
        public PickupAreaLogic(IPickupAreaRepository pickupAreaRepository)
        {
            _pickupAreaRepository = pickupAreaRepository;
        }
        public bool Add(PickupArea pickupArea)
        {
            bool isAdded = _pickupAreaRepository.Add(pickupArea);
            return isAdded;
        }
        public bool Delete(int pickupAreaNumber) 
        {
            Func<PickupArea, bool> getPickupAreaNumber = getPickupAreaNumber => getPickupAreaNumber.Id == pickupAreaNumber;
            bool isDeleted = _pickupAreaRepository.Delete(getPickupAreaNumber);
            return isDeleted;
        }
    }
}
