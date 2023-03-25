using Microsoft.EntityFrameworkCore;
using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
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
        public int? AddReturnId(GeoPoint geoPoint)
        {
            try
            {
                int? isAdded = _geoPointRepository.AddReturnId(geoPoint);
                return isAdded;
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException(Error.AlreadyFound);
            }
           
        }
        public async Task<bool> AddAsync(GeoPoint geoPoint)
        {
            try
            {
                bool isAdded = await _geoPointRepository.AddAsync(geoPoint);
                return isAdded;
            }
            catch(DbUpdateException ex)
            {
                throw new DbUpdateException(Error.AlreadyFound);
            }
                
        }
        public bool Delete(int geoPointNumber) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<GeoPoint, bool> getGeoPointNumber = getGeoPointNumber => getGeoPointNumber.Id == geoPointNumber;
            bool isDeleted = _geoPointRepository.Delete(getGeoPointNumber);
            return isDeleted;
        }
        public GeoPoint? Find(int geoPointId)
        {
            Func<GeoPoint, bool> getGeoPointId = getGeoPointId => getGeoPointId.Id == geoPointId;       
            GeoPoint? isFound = _geoPointRepository.GetSingle(getGeoPointId);
            return isFound;
        }
        public List<GeoPoint>? GetAll()
        {
            List<GeoPoint>? geoPointList = _geoPointRepository.Get();
            return geoPointList;
        }
        public int? FindByCoordinate(string longitude,string latitude)
        {
            Func<GeoPoint, bool> getGeoPointLongitude = getGeoPointLongitude => getGeoPointLongitude.Longtitude == longitude;
            Func<GeoPoint, bool> getGeoPointLatitude = getGeoPointLatitude => getGeoPointLatitude.Latitude == latitude;
            int? isFound = _geoPointRepository.GetId(getGeoPointLongitude, getGeoPointLatitude);
            return isFound;
        }
        public GeoPoint? FindByCoordinateGeoPoint(string longitude, string latitude)
        {
            Func<GeoPoint, bool> getGeoPointLongitude = getGeoPointLongitude => getGeoPointLongitude.Longtitude == longitude;
            Func<GeoPoint, bool> getGeoPointLatitude = getGeoPointLatitude => getGeoPointLatitude.Latitude == latitude;
            GeoPoint? isFound = _geoPointRepository.GetSingle(getGeoPointLongitude, getGeoPointLatitude);
            return isFound;
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
