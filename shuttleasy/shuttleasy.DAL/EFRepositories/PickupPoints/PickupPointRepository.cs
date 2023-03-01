using shuttleasy.DAL.EFRepositories.PickupAreas;
using shuttleasy.DAL.Interfaces;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.EFRepositories.PickupPoints
{
    public class PickupPointRepository : Repository<PickupPoint>, IPickupPointRepository
    {
    }
}
