using CPTestProject.Data;
using Microsoft.EntityFrameworkCore;

namespace CPTestProject.Services.Endpoints
{
    public static class MapEndpoint
    {
        public static void MapDataEndpoints(this WebApplication app)
        {
            // Return available imported files
            app.MapGet("/api/map/imports", async (AppDbContext db) =>
            {
                var imports = await db.ImportBatches
                    .AsNoTracking() //No tracking for read-only queries
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .Select(x => new
                    {
                        x.BatchId,
                        x.SourceFileName,
                        x.FileType,
                        x.CreatedAtUtc
                    })
                    .ToListAsync();

                return Results.Ok(imports);
            });

            // GPS coordinates for selected import
            app.MapGet("/api/map/gps/{batchId:guid}", async (Guid batchId, AppDbContext db) =>
            {
                var points = await db.GpsRecords
                    .AsNoTracking() //Again, no tracking for read-only queries
                    .Where(x => x.IdBatch == batchId)
                    .OrderBy(x => x.TimestampUtc)
                    .Select(x => new
                    {
                        x.Latitude,
                        x.Longitude,
                        x.TimestampUtc
                    })
                    .ToListAsync();
                // Equivalent for T-SQL Select Latitude, Longitude, TimestampUtc from GpsRecords where IdBatch=batchId order by TimestampUtc
                return Results.Ok(points);
            });

            // BTS observations for selected import
            app.MapGet("/api/map/bts/{batchId:guid}", async (Guid batchId, AppDbContext db) =>
            {
                var observations = await db.BtsRecords
                    .AsNoTracking() //<< third time
                    .Where(x => x.IdBatch == batchId)
                    .OrderBy(x => x.TimestampUtc)
                    .Select(x => new
                    {
                        x.Id,
                        x.TimestampUtc,
                        x.Latitude,
                        x.Longitude,
                        x.Accuracy,
                        x.Mcc,
                        x.Mnc,
                        x.Lac,
                        x.CellId,
                        x.ShortCellId,
                        x.NetworkType,
                        x.IsNeighboring,
                        x.Rnc,
                        x.Psc,
                        x.Arfcn,
                        x.SignalDbm,
                        x.Rsrp,
                        x.Rsrq,
                        x.Rssi
                    })
                    .ToListAsync();

                return Results.Ok(observations);
            });
        }
    }
}
