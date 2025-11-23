using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ca_aggregates",
                columns: table => new
                {
                    CaAddress = table.Column<string>(type: "character varying(44)", maxLength: 44, nullable: false),
                    MentionCount5m = table.Column<double>(type: "double precision", nullable: false),
                    MentionCount1h = table.Column<double>(type: "double precision", nullable: false),
                    MentionCount6h = table.Column<double>(type: "double precision", nullable: false),
                    MentionCount24h = table.Column<double>(type: "double precision", nullable: false),
                    TrendScore = table.Column<double>(type: "double precision", nullable: false),
                    LastMentioned = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastCalculated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmaValuesJson = table.Column<string>(type: "jsonb", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    EnteredTop25At = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ca_aggregates", x => x.CaAddress);
                });

            migrationBuilder.CreateTable(
                name: "ca_mention_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CaAddress = table.Column<string>(type: "character varying(44)", maxLength: 44, nullable: false),
                    TweetId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Followers = table.Column<int>(type: "integer", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsReply = table.Column<bool>(type: "boolean", nullable: false),
                    IsQuote = table.Column<bool>(type: "boolean", nullable: false),
                    IsRetweet = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ca_mention_records", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "ix_ca_mention_records_ca_timestamp",
                table: "ca_mention_records",
                columns: new[] { "CaAddress", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "ix_ca_mention_records_tweet_ca_unique",
                table: "ca_mention_records",
                columns: new[] { "TweetId", "CaAddress" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ca_aggregates");

            migrationBuilder.DropTable(
                name: "ca_mention_records");
        }
    }
}
