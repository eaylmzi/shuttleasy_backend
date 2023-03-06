using System.Globalization;
using System;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Collections;

namespace shuttleasy.Route
{
    public class Routes
    {
        private double stringToDouble(string number)
        {
            double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out double myDouble);
            return myDouble;
        }
        public double calculateDistance(Location loc1 , Location loc2)
        {
            double loc1longitude = stringToDouble(loc1.Longitude);
            double loc1latitude = stringToDouble(loc1.Latitude);
            double loc2longitude = stringToDouble(loc2.Longitude);
            double loc2latitude = stringToDouble(loc2.Latitude);
            double distance = Math.Sqrt(Math.Pow(loc2latitude - loc1latitude,2)+ Math.Pow(loc2longitude - loc1longitude, 2));

            return distance;
        }
        public double[,] listDistanceBetweenPoints(List<Location> points)
        {
            double[,] distanceArray = new double[points.Count, points.Count];
            for(int i = 0;points.Count > i; i++)
            {
                for (int j = 0; points.Count > j; j++)
                {
                    distanceArray[i,j] = calculateDistance(points[i], points[j]);
                }

            }
            
            return distanceArray;
        } 
        public void shortestPath(double[,] distanceArray)
        {

        }
    }
    public class Location
    {
        public string Longitude { get; set; } = null!;
        public string Latitude { get; set; } = null!;
    }
}
