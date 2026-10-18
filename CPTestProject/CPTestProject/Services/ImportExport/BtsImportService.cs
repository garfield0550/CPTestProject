using CPTestProject.Model;
using CsvHelper;
using System.Globalization;

namespace CPTestProject.Services.ImportExport
{
    public class BtsImportService
    {
        public async Task<List<BtsRecord>> ParseAsync(
            Stream stream, Guid batchId)
        {
            var btsRecords = new List<BtsRecord>();

            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            if (!await csv.ReadAsync())
                throw new InvalidDataException("CSV soubor je prázdný.");

            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                try
                {
                    // Required fields
                    double latitude = csv.GetField<double>("lat");
                    double longitude = csv.GetField<double>("lon");

                    if (!double.IsFinite(latitude) || !double.IsFinite(longitude) || latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                        throw new InvalidDataException("Neplatné GPS souřadnice.");

                    string timestampText = csv.GetField("measured_at") ?? throw new InvalidDataException("Chybí čas měření.");

                    if (!DateTimeOffset.TryParse(timestampText, CultureInfo.InvariantCulture, DateTimeStyles.None, out var timestamp))
                        throw new InvalidDataException("Neplatný čas měření.");

                    var record = new BtsRecord
                    {
                        Id = Guid.NewGuid(),
                        IdBatch = batchId,

                        // GPS information
                        TimestampUtc = timestamp.UtcDateTime,
                        Latitude = latitude,
                        Longitude = longitude,
                        Accuracy = csv.GetField<double?>("accuracy"),

                        // Network identification
                        Mcc = csv.GetField<int>("mcc"),
                        Mnc = csv.GetField<int>("mnc"),
                        Lac = csv.GetField<int?>("lac"),
                        CellId = csv.GetField<long>("cell_id"),
                        ShortCellId = csv.GetField<long?>("short_cell_id"),

                        NetworkType = csv.GetField("net_type") ?? string.Empty,
                        IsNeighboring = csv.GetField<bool>("neighboring"),

                        // Additional identification
                        Rnc = csv.GetField<int?>("rnc"),
                        Psc = csv.GetField<int?>("psc"),
                        Arfcn = csv.GetField<int?>("arfcn"),

                        // Signal measurements
                        SignalDbm = csv.GetField<int?>("dbm"),
                        Rsrp = csv.GetField<int?>("rsrp"),
                        Rsrq = csv.GetField<int?>("rsrq"),
                        Rssi = csv.GetField<int?>("rssi")
                    };

                    if (string.IsNullOrWhiteSpace(record.NetworkType))
                    {
                        throw new InvalidDataException("Chybí typ mobilní sítě.");
                    }

                    btsRecords.Add(record);
                }
                catch (Exception ex) when (ex is CsvHelperException || ex is FormatException || ex is InvalidDataException || ex is OverflowException)
                {
                    throw new InvalidDataException($"Chyba při zpracování CSV na řádku {csv.Context.Parser.Row}: {ex.Message}", ex);
                }
            }

            return btsRecords;
        }
    }
}
