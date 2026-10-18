using CPTestProject.Model;
using System.Globalization;
using System.Xml.Linq;

namespace CPTestProject.Services.ImportExport
{
    public class GpsImportService
    {

        public async Task<List<GpsRecord>> ParseAsync(
            Stream stream, Guid batchId)
        {
            var gpsRecords = new List<GpsRecord>();
            var document = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);

            XNamespace ns = document.Root?.Name.Namespace ?? throw new InvalidDataException("Chybný GPX dokument.");

            if (document.Root?.Name.LocalName != "gpx")
            {
                throw new InvalidDataException(
                    "Soubor není platný dokument GPX pro GPS Logger.");
            }

            foreach (var point in document.Descendants(ns + "trkpt"))
            {
                var latText = point.Attribute("lat")?.Value;
                var lonText = point.Attribute("lon")?.Value;
                var timeText = point.Element(ns + "time")?.Value;

                if (!double.TryParse(latText, NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude) ||
                    !double.TryParse(lonText, NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude) ||
                    !DateTimeOffset.TryParse(timeText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var timestamp))
                {
                    continue;
                }

                if (!double.IsFinite(latitude) || !double.IsFinite(longitude) || latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                {
                    continue;
                }

                double? ParseOptionalDouble(string name)
                {
                    var value = point.Element(ns + name)?.Value;

                    return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : null;
                }

                int? ParseOptionalInt(string name)
                {
                    var value = point.Element(ns + name)?.Value;

                    return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : null;
                }

                gpsRecords.Add(new GpsRecord
                {
                    Id = Guid.NewGuid(),
                    IdBatch = batchId,
                    Latitude = latitude,
                    Longitude = longitude,
                    Altitude = ParseOptionalDouble("ele"),
                    Speed = ParseOptionalDouble("speed"),
                    SatelliteCount = ParseOptionalInt("sat"),
                    TimestampUtc = timestamp.UtcDateTime
                });
            }

            return gpsRecords;
        }
    }

}

