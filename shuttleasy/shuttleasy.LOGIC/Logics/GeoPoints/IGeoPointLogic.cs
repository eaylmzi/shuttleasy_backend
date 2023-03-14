using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.GeoPoints
{
    public interface IGeoPointLogic
    {
        public bool Add(GeoPoint geoPoint);
        public Task<bool> AddAsync(GeoPoint geoPoint);
        public bool Delete(int geoPointNumber);
        public List<GeoPoint>? GetAll();
      //  public Task<GeoPoint?> GetGeoPointWithLocationName(string locationName);

    }
}
