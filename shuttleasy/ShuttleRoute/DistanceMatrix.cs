namespace ShuttleRoute
{
    public class DistanceMatrix
    {
        public string Code { get; set; }
        public double[][] Distances { get; set; }
        public Location[] Destinations { get; set; }
        public double[][] Durations { get; set; }
        public Location[] Sources { get; set; }
    }
}
