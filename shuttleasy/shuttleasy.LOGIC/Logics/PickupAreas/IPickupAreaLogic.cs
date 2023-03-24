using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.PickupAreas
{
    public interface IPickupAreaLogic
    {
        public bool Add(PickupArea pickupArea);
        public bool Delete(int pickupAreaNumber);
        public PickupArea? Find(int sessionId);
        public bool DeleteBySessionId(int sessionId);
    }
}
