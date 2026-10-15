namespace CPTestProject.Model
{
    public class BtsRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid IdBatch { get; set; }
        public DateTime TimestampUtc { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Accuracy { get; set; }

        // Mobile Network Code (MNC) and Mobile Country Code (MCC)
        public int Mcc { get; set; }
        public int Mnc { get; set; }

        // Location Area Code (LAC) and Cell ID
        public int? Lac { get; set; }
        public long CellId { get; set; }
        public long? ShortCellId { get; set; }

        // Network type (e.g., GSM, UMTS, LTE)
        public string NetworkType { get; set; } = string.Empty;
        public bool IsNeighboring { get; set; }

        // Technology-dependent identifiers from Tower Collector. (Presumably Local Cell ID (RNC), Primary Scrambling Code (PSC), Absolute Radio Frequency Channel Number (ARFCN))
        public int? Rnc { get; set; }
        public int? Psc { get; set; }
        public int? Arfcn { get; set; }

        // Signal strength metrics
        public int? SignalDbm { get; set; }
        public int? Rsrp { get; set; }
        public int? Rsrq { get; set; }
        public int? Rssi { get; set; }


    }
}
