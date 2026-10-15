namespace CPTestProject.Model
{
    public class GpsRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid IdBatch { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? Speed { get; set; }
        public int? SatelliteCount { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
