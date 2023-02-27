﻿using shuttleasy.DAL.EFRepositories.Destinations;
using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool Delete(int geoPointNumber) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<GeoPoint, bool> getGeoPointNumber = getGeoPointNumber => getGeoPointNumber.Id == geoPointNumber;
            bool isDeleted = _geoPointRepository.Delete(getGeoPointNumber);
            return isDeleted;
        }
    }
}