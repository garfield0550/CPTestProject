// Created using the .NET CLI command: dotnet ef migrations add InitialCreate

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPTestProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImportBatches",
                columns: table => new
                {
                    BatchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SourceFileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportBatches", x => x.BatchId);
                });

            migrationBuilder.CreateTable(
                name: "BtsRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdBatch = table.Column<Guid>(type: "TEXT", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Accuracy = table.Column<double>(type: "REAL", nullable: true),
                    Mcc = table.Column<int>(type: "INTEGER", nullable: false),
                    Mnc = table.Column<int>(type: "INTEGER", nullable: false),
                    Lac = table.Column<int>(type: "INTEGER", nullable: true),
                    CellId = table.Column<long>(type: "INTEGER", nullable: false),
                    ShortCellId = table.Column<long>(type: "INTEGER", nullable: true),
                    NetworkType = table.Column<string>(type: "TEXT", nullable: false),
                    IsNeighboring = table.Column<bool>(type: "INTEGER", nullable: false),
                    Rnc = table.Column<int>(type: "INTEGER", nullable: true),
                    Psc = table.Column<int>(type: "INTEGER", nullable: true),
                    Arfcn = table.Column<int>(type: "INTEGER", nullable: true),
                    SignalDbm = table.Column<int>(type: "INTEGER", nullable: true),
                    Rsrp = table.Column<int>(type: "INTEGER", nullable: true),
                    Rsrq = table.Column<int>(type: "INTEGER", nullable: true),
                    Rssi = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BtsRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BtsRecords_ImportBatches_IdBatch",
                        column: x => x.IdBatch,
                        principalTable: "ImportBatches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GpsRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdBatch = table.Column<Guid>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Altitude = table.Column<double>(type: "REAL", nullable: true),
                    Speed = table.Column<double>(type: "REAL", nullable: true),
                    SatelliteCount = table.Column<int>(type: "INTEGER", nullable: true),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpsRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GpsRecords_ImportBatches_IdBatch",
                        column: x => x.IdBatch,
                        principalTable: "ImportBatches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BtsRecords_IdBatch_TimestampUtc",
                table: "BtsRecords",
                columns: new[] { "IdBatch", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_GpsRecords_IdBatch_TimestampUtc",
                table: "GpsRecords",
                columns: new[] { "IdBatch", "TimestampUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BtsRecords");

            migrationBuilder.DropTable(
                name: "GpsRecords");

            migrationBuilder.DropTable(
                name: "ImportBatches");
        }
    }
}
