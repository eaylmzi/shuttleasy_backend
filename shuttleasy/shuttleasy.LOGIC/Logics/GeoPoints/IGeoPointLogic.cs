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
        public int? AddReturnId(GeoPoint geoPoint);
        public Task<bool> AddAsync(GeoPoint geoPoint);
        public bool Delete(int geoPointNumber);
        public List<GeoPoint>? GetAll();
        public GeoPoint? Find(int geoPointId);
        public int? FindByCoordinate(string longitude, string latitude);
        public GeoPoint? FindByCoordinateGeoPoint(string longitude, string latitude);
      //  public Task<GeoPoint?> GetGeoPointWithLocationName(string locationName);

    }
}
