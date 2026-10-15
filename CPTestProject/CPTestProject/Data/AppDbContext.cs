using CPTestProject.Model;
using Microsoft.EntityFrameworkCore;

namespace CPTestProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ImportBatch> ImportBatches { get; set; }
        public DbSet<GpsRecord> GpsRecords { get; set; }
        public DbSet<BtsRecord> BtsRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ImportBatch>()
                .HasKey(x => x.BatchId);

            modelBuilder.Entity<GpsRecord>()
                .HasOne<ImportBatch>()
                .WithMany()
                .HasForeignKey(x => x.IdBatch)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BtsRecord>()
                .HasOne<ImportBatch>()
                .WithMany()
                .HasForeignKey(x => x.IdBatch)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GpsRecord>()
                .HasIndex(x => new { x.IdBatch, x.TimestampUtc });

            modelBuilder.Entity<BtsRecord>()
                .HasIndex(x => new { x.IdBatch, x.TimestampUtc });
        }

    }
}
