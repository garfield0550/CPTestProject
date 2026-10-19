namespace CPTestProject.Model
{
    public class ImportBatch
    {
        public Guid BatchId { get; set; } = Guid.NewGuid();

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string SourceFileName { get; set; } = string.Empty;
        public ImportedFileType FileType { get; set; }
    }

    public enum ImportedFileType
    {
        GPS = 1,
        BTS = 2
    }
}
