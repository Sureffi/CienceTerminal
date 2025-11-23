using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLegacyCaAggregatesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ca_aggregates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ca_aggregates",
                columns: table => new
                {
                    CaAddress = table.Column<string>(type: "character varying(44)", maxLength: 44, nullable: false),
                    EmaValuesJson = table.Column<string>(type: "jsonb", nullable: false),
                    EnteredTop25At = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastCalculated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastMentioned = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MentionCount1h = table.Column<double>(type: "double precision", nullable: false),
                    MentionCount24h = table.Column<double>(type: "double precision", nullable: false),
                    MentionCount5m = table.Column<double>(type: "double precision", nullable: false),
                    MentionCount6h = table.Column<double>(type: "double precision", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    TrendScore = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ca_aggregates", x => x.CaAddress);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ca_aggregates_rank",
                table: "ca_aggregates",
                column: "Rank",
                filter: "\"Rank\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_ca_aggregates_trend_score",
                table: "ca_aggregates",
                column: "TrendScore",
                descending: new bool[0]);
        }
    }
}
