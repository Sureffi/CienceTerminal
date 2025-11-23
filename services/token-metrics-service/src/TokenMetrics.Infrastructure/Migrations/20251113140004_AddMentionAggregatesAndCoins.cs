using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMentionAggregatesAndCoins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "coins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoinMintAddress = table.Column<string>(type: "character varying(44)", maxLength: 44, nullable: false),
                    CoinSymbol = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CoinImage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MentionCount24h = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    HolderCount = table.Column<int>(type: "integer", nullable: true),
                    Liquidity = table.Column<decimal>(type: "numeric(28,8)", precision: 28, scale: 8, nullable: true),
                    Volume24h = table.Column<decimal>(type: "numeric(28,8)", precision: 28, scale: 8, nullable: true),
                    MarketCap = table.Column<decimal>(type: "numeric(28,8)", precision: 28, scale: 8, nullable: true),
                    TopHoldersPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    FirstPoolCreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsBlacklisted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mention_aggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoinMintAddress = table.Column<string>(type: "character varying(44)", maxLength: 44, nullable: false),
                    MentionCount5m = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    MentionCount1h = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    MentionCount6h = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    MentionCount24h = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    TrendingScore = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    LastMentioned = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    LastCalculated = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mention_aggregates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_coins_last_updated",
                table: "coins",
                column: "LastUpdated");

            migrationBuilder.CreateIndex(
                name: "ix_coins_mention_count",
                table: "coins",
                column: "MentionCount24h",
                descending: new bool[0],
                filter: "\"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "ix_coins_mint_address",
                table: "coins",
                column: "CoinMintAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_mention_aggregates_coin_address",
                table: "mention_aggregates",
                column: "CoinMintAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_mention_aggregates_rank",
                table: "mention_aggregates",
                column: "Rank",
                filter: "\"Rank\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_mention_aggregates_trending_score",
                table: "mention_aggregates",
                column: "TrendingScore",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coins");

            migrationBuilder.DropTable(
                name: "mention_aggregates");
        }
    }
}
