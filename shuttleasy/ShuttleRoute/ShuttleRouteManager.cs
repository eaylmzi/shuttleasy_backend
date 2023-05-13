using Newtonsoft.Json;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto.Session.dto;

namespace ShuttleRoute
{
    // class to calculate the optimal shuttle route with using MapBox Matrix API and Dijkstra Algorithm.
    public static class ShuttleRouteManager
    {
        private static string accessToken = "pk.eyJ1IjoiY2VtcmViaXRnZW4iLCJhIjoiY2xlaW9sajJ4MDNpZjNxazU4bDljYmYyeCJ9.9Ue3eCQ1QJPIrKxGqQOY-A";
        private static string matrixApiUrl = "https://api.mapbox.com/directions-matrix/v1/mapbox/driving/";
        private static HttpClient client = new HttpClient();

        static async Task<DistanceMatrix?> CreateMatrix(string[] coordinates)
        {
            var req = $"{matrixApiUrl}{string.Join(";", coordinates)}?annotations=distance,duration&access_token={accessToken}";
            var response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                try
                {
                    var distanceMatrix = JsonConvert.DeserializeObject<DistanceMatrix>(responseBody);
                    return distanceMatrix;
                }
                catch (Exception)
                {
                    return new DistanceMatrix();
                }
            }
            return null;

        }

        static string StringfyGeoLocations(string latitude, string longitude)
        {
            var geoLocation = $"{longitude},{latitude}";
            return (geoLocation);
        }

        static string[] GetGeoPointList(ShuttleManager shuttleManager)
        {
            var geoPointList = new string[shuttleManager.PassengerRouteDto.Count + 1];

            geoPointList[0] = (StringfyGeoLocations(shuttleManager.ShuttleRouteDto.StartGeopoint.Latitude,
                shuttleManager.ShuttleRouteDto.StartGeopoint.Longtitude));

            int cnt = 1;

            shuttleManager.PassengerRouteDto.ForEach(passenger =>
            {
                geoPointList[cnt] = (StringfyGeoLocations(passenger.Latitude, passenger.Longtitude));
                cnt++;
            });
            return geoPointList;
        }

        static async Task<List<PassengerRouteDto>> SetSortedPassengerList(int[] sortedIndices, List<PassengerRouteDto> inputPassengerList, DateTime date, double[][] durations, GeoPoint final)
        {
            var sortedList = new List<PassengerRouteDto>();
            var startIndex = 0;
            var passedSecs = new List<double>();
            var lastVisitedIndex = sortedIndices[sortedIndices.Length - 1];
            double totalSecs = 0;
            for (var i = 0; i < sortedIndices.Length; i++)
            {
                if (sortedIndices[i] != startIndex)
                {
                    var passenger = inputPassengerList.ElementAt(sortedIndices[i] - 1);
                    sortedList.Add(passenger);
                }
            }
            for (var i = sortedList.Count - 1; i > -1; i--)
            {
                totalSecs += durations[lastVisitedIndex][sortedIndices[i]];
                passedSecs.Insert(0, totalSecs);
                lastVisitedIndex = sortedIndices[i];
            }

            //get final duration
            var finalPointMatrix = await CreateMatrix(new string[]
            {
                $"{sortedList.ElementAt(sortedList.Count-1).Longtitude},{sortedList.ElementAt(sortedList.Count-1).Latitude}",
                $"{final.Longtitude},{final.Latitude}"
            });
            var lastDuration = finalPointMatrix != null ? finalPointMatrix.Durations[0][1] : 0;
            for (var i = 0; i < passedSecs.Count; i++)
            {
                sortedList.ElementAt(i).EstimatedArriveTime = date.AddSeconds(-(passedSecs.ElementAt(i) + lastDuration));
            }
            return sortedList;
        }
        public static async Task<ShuttleManager> CalculateRouteAsync(ShuttleManager input)
        {
            var geoPointList = GetGeoPointList(input);

            var matrix = await CreateMatrix(geoPointList);

            if (matrix != null)
            {
                var graph = new Graph();
                var l = graph.DijkstraAlgorithm(matrix.Distances);
                var sortedList =
                   await SetSortedPassengerList(l, input.PassengerRouteDto, input.ShuttleRouteDto.StartTime, matrix.Durations, input.ShuttleRouteDto.FinalGeopoint);
                var shuttleManager = new ShuttleManager
                {
                    PassengerRouteDto = sortedList,
                    ShuttleRouteDto = input.ShuttleRouteDto
                };

                return shuttleManager;
            }
            return new ShuttleManager();
        }

    }
}
