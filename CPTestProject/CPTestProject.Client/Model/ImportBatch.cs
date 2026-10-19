namespace CPTestProject.Client.Model
{
    // Batches mainly in use for dropdown selection of imported files. FileType decides which dropdown to populate.
    public class ImportBatch
    {
        public Guid BatchId { get; set; }
        public string SourceFileName { get; set; } = string.Empty;
        public int FileType { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
