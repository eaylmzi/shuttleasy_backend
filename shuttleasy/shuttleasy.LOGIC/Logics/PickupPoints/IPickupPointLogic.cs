using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.PickupPoints
{
    public interface IPickupPointLogic
    {
        public bool Add(PickupPoint pickupPoint);
        public int? AddReturnId(PickupPoint pickupPoint);
        public bool Delete(int pickupPointNumber);
        public List<PickupPoint>? GetListById(int pickupId);
        public PickupPoint? GetSingle(int pickupId);
    }
}
