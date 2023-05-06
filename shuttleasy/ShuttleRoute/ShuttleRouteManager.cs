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

        //  Creating Matrix with the real world coordinates with MapBox Matrix API.
        static async Task<DistanceMatrix?> CreateMatrix(string[] coordinates)
        {
            var req = $"{matrixApiUrl}{string.Join(";", coordinates)}?annotations=distance,duration&access_token={accessToken}";
            var response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var distanceMatrix = JsonConvert.DeserializeObject<DistanceMatrix>(responseBody);
                return distanceMatrix;
            }
            return null;
        }

        // Helper function for stringfying geo location for request body.
        static string StringfyGeoLocations(string latitude, string longitude)
        {
            var geoLocation = $"{longitude},{latitude}";
            return (geoLocation);
        }

        //creating geo point list from the input for request body.
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

        // creating sorted PassengerRouteDto list for the output.
        static List<PassengerRouteDto> SetSortedPassengerList(int[] sortedIndices, List<PassengerRouteDto> inputPassengerList)
        {
            var sortedList = new List<PassengerRouteDto>();
            var startIndex = 0;
            for (var i = 0; i < sortedIndices.Length; i++)
            {
                if (sortedIndices[i] != startIndex)
                {
                    sortedList.Add(inputPassengerList.ElementAt(sortedIndices[i] - 1));
                }
            }
            return sortedList;
        }

        // the main function to calculate optimal path. It uses Dijkstra Algorithm to calculate optimal path.
        public static async Task<ShuttleManager> CalculateRouteAsync(ShuttleManager input)
        {
            var geoPointList = GetGeoPointList(input);

            var matrix = await CreateMatrix(geoPointList);

            if (matrix != null)
            {
                var graph = new Graph();
                var l = graph.DijkstraAlgorithm(matrix.Distances);
                var sortedList = SetSortedPassengerList(l, input.PassengerRouteDto);
                var shuttleManager = new ShuttleManager();
                shuttleManager.PassengerRouteDto = sortedList;
                shuttleManager.ShuttleRouteDto = input.ShuttleRouteDto;

                return shuttleManager;
            }
            return new ShuttleManager();
        }
    }
}
