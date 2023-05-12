using Newtonsoft.Json;
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
            Console.WriteLine($"Error: {response.StatusCode}");
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



        static List<PassengerRouteDto> SetSortedPassengerList(int[] sortedIndices, List<PassengerRouteDto> inputPassengerList, DateTime date, double[][] durations)
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
            for (var i = sortedList.Count - 1; i >= 0; i--)
            {
                if (sortedIndices[i] != startIndex)
                {
                    totalSecs += durations[lastVisitedIndex][sortedIndices[i]];
                    passedSecs.Insert(0, totalSecs);
                    lastVisitedIndex = sortedIndices[i];
                }
            }
            for (var i = 0; i < passedSecs.Count; i++)
            {
                sortedList.ElementAt(i).EstimatedArriveTime = date.AddSeconds(-(passedSecs.ElementAt(i)));
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
                var sortedList = SetSortedPassengerList(l, input.PassengerRouteDto, input.ShuttleRouteDto.StartTime, matrix.Durations);
                var shuttleManager = new ShuttleManager();
                shuttleManager.PassengerRouteDto = sortedList;
                shuttleManager.ShuttleRouteDto = input.ShuttleRouteDto;



                return shuttleManager;
            }
            return new ShuttleManager();
        }



    }
}
