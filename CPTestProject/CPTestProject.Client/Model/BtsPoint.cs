namespace CPTestProject.Client.Model
{ 
    // this time Ids are needed for map markers and details opposed to simple line of GPS points.
    public class BtsPoint
    {
        public Guid Id { get; set; }
        public DateTime TimestampUtc { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Accuracy { get; set; }

        public int Mcc { get; set; }
        public int Mnc { get; set; }
        public int? Lac { get; set; }
        public long CellId { get; set; }
        public long? ShortCellId { get; set; }

        public string NetworkType { get; set; } = "";
        public bool IsNeighboring { get; set; }

        public int? Rnc { get; set; }
        public int? Psc { get; set; }
        public int? Arfcn { get; set; }

        public int? SignalDbm { get; set; }
        public int? Rsrp { get; set; }
        public int? Rsrq { get; set; }
        public int? Rssi { get; set; }
    }
}
