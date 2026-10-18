using CPTestProject.Data;
using CPTestProject.Model;
using CPTestProject.Services.ImportExport;
using Microsoft.EntityFrameworkCore;

namespace CPTestProject.Services.ImportExport
{
    public class ImportService
    {
        private readonly AppDbContext Db;
        private readonly GpsImportService GpsParser;
        private readonly BtsImportService BtsParser;
        


        public ImportService(AppDbContext db, GpsImportService gpsParser, BtsImportService btsParser)
        {
            Db = db;
            GpsParser = gpsParser;
            BtsParser = btsParser;
        }

        public async Task<(Guid BatchId, int RecordCount)> ImportAsync(Stream stream, string fileName, ImportedFileType fileType, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stream);

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Chybí název souboru.", nameof(fileName));

            var batch = new ImportBatch
            {
                BatchId = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                SourceFileName = Path.GetFileName(fileName),
                FileType = fileType
            };

            switch (fileType)
            {
                case ImportedFileType.GPS:
                    {
                        var records = await GpsParser.ParseAsync(stream, batch.BatchId);

                        if (records.Count == 0)
                            throw new InvalidDataException("GPX neobsahuje žádné platné záznamy.");

                        Db.ImportBatches.Add(batch);
                        Db.GpsRecords.AddRange(records);
                        break;
                    }

                case ImportedFileType.BTS:
                    {
                        var records = await BtsParser.ParseAsync(stream, batch.BatchId);

                        if (records.Count == 0)
                            throw new InvalidDataException("CSV neobsahuje žádné platné záznamy.");

                        Db.ImportBatches.Add(batch);
                        Db.BtsRecords.AddRange(records);

                        break;
                    }

                default:
                    throw new NotSupportedException("Nepodporovaný typ importu.");
            }

            await Db.SaveChangesAsync(cancellationToken);

            int recordCount = fileType switch
            {
                ImportedFileType.GPS => Db.ChangeTracker.Entries<GpsRecord>().Count(e => e.Entity.IdBatch == batch.BatchId),
                ImportedFileType.BTS => Db.ChangeTracker.Entries<BtsRecord>().Count(e => e.Entity.IdBatch == batch.BatchId),
                _ => 0
            };

            return (batch.BatchId, recordCount);
        }
    }
}
