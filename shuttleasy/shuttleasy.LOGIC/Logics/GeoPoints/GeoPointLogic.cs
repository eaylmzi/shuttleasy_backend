using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace shuttleasy.LOGIC.Logics.GeoPoints
{
    public class GeoPointLogic : IGeoPointLogic
    {
        private IGeoPointRepository _geoPointRepository;
        public GeoPointLogic(IGeoPointRepository geoPointRepository)
        {
            _geoPointRepository = geoPointRepository;
        }
        public bool Add(GeoPoint geoPoint)
        {
            bool isAdded = _geoPointRepository.Add(geoPoint);
            return isAdded;
        }
        public async Task<bool> AddAsync(GeoPoint geoPoint)
        {
            bool isAdded = await _geoPointRepository.AddAsync(geoPoint);
            return isAdded;
        }
        public bool Delete(int geoPointNumber) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<GeoPoint, bool> getGeoPointNumber = getGeoPointNumber => getGeoPointNumber.Id == geoPointNumber;
            bool isDeleted = _geoPointRepository.Delete(getGeoPointNumber);
            return isDeleted;
        }
        public List<GeoPoint>? GetAll()
        {
            List<GeoPoint>? geoPointList = _geoPointRepository.Get();
            return geoPointList;
        }
        /*
        public async Task<GeoPoint?> GetGeoPointWithLocationName(string locationName) 
        {
            Func<GeoPoint, bool> getGeoPoint = getGeoPoint => getGeoPoint.LocationName == locationName;
            GeoPoint? geoPoint =  await _geoPointRepository.GetSingleAsync(entity => entity.LocationName == locationName);
            return geoPoint;

        }
        */
    }
}
