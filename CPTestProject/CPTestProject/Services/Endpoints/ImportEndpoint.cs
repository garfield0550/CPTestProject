using CPTestProject.Model;
using CPTestProject.Services.ImportExport;

namespace CPTestProject.Services.Endpoints
{
    public static class ImportEndpoint
    {
        public static void MapImportEndpoints(this WebApplication app)
        {
            // Common endpoint for importing files. The type of import is determined by the 'type' route parameter. Request format should be multipart/form-data.
            app.MapPost("/api/import/{type}", async (string type, IFormFile file, ImportService importService, CancellationToken cancellationToken) => 
            {
                ImportedFileType fileType;
                switch (type.ToLowerInvariant())
                {
                    case "gps":
                        fileType = ImportedFileType.GPS;
                        break;

                    case "bts":
                        fileType = ImportedFileType.BTS;
                        break;

                    default:
                        return Results.BadRequest("Neznámý typ importu.");
                }

                if (file.Length == 0) 
                    return Results.BadRequest("Soubor je prázdný.");

                // 1 * 1024 = 1KB * 1024 = 1 MB * 20 = 20 MB. Also check server configuration for max request size (e.g., Kestrel's MaxRequestBodySize or IIS's maxAllowedContentLength).
                if (file.Length > 20 * 1024 * 1024)
                    return Results.BadRequest("Soubor překračuje maximální velikost 20 MB.");

                try
                {
                    await using var stream = file.OpenReadStream();
                    var result = await importService.ImportAsync(stream, file.FileName, fileType, cancellationToken);
                    return Results.Ok(new { result.BatchId, result.RecordCount });
                }
                catch (InvalidDataException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .DisableAntiforgery(); // Disabled for preview purposes. In production, consider enabling CSRF protection.
        }
    }
}
