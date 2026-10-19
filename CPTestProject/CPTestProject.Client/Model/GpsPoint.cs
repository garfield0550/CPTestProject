namespace CPTestProject.Client.Model
{
    //No need for IDs to show on the map, so we declare just the coordinates and timestamp for ordering.
    public class GpsPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
