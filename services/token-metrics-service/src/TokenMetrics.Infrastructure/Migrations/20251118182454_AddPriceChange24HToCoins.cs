using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceChange24HToCoins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Volume24h",
                table: "coins",
                type: "double precision",
                precision: 28,
                scale: 8,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(28,8)",
                oldPrecision: 28,
                oldScale: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TopHoldersPercentage",
                table: "coins",
                type: "double precision",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "MarketCap",
                table: "coins",
                type: "double precision",
                precision: 28,
                scale: 8,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(28,8)",
                oldPrecision: 28,
                oldScale: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Liquidity",
                table: "coins",
                type: "double precision",
                precision: 28,
                scale: 8,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(28,8)",
                oldPrecision: 28,
                oldScale: 8,
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PriceChange24H",
                table: "coins",
                type: "double precision",
                precision: 10,
                scale: 4,
                nullable: true);

            // Skip alerts table creation - it's owned by Alert Service and already exists
            // migrationBuilder.CreateTable(
            //     name: "alerts",
            //     columns: table => new
            //     {
            //         Id = table.Column<Guid>(type: "uuid", nullable: false),
            //         AlertType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //         CoinMintAddress = table.Column<string>(type: "character varying(44)", maxLength: 44, nullable: true),
            //         CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_alerts", x => x.Id);
            //     });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Skip alerts table drop - it's owned by Alert Service
            // migrationBuilder.DropTable(
            //     name: "alerts");

            migrationBuilder.DropColumn(
                name: "PriceChange24H",
                table: "coins");

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume24h",
                table: "coins",
                type: "numeric(28,8)",
                precision: 28,
                scale: 8,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 28,
                oldScale: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TopHoldersPercentage",
                table: "coins",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MarketCap",
                table: "coins",
                type: "numeric(28,8)",
                precision: 28,
                scale: 8,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 28,
                oldScale: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Liquidity",
                table: "coins",
                type: "numeric(28,8)",
                precision: 28,
                scale: 8,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 28,
                oldScale: 8,
                oldNullable: true);
        }
    }
}
