using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlertType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AlertData = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CoinMintAddress = table.Column<string>(type: "character varying(44)", maxLength: 44, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_alerts_coin_mint_address",
                table: "alerts",
                column: "CoinMintAddress",
                filter: "\"CoinMintAddress\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_alerts_created_at",
                table: "alerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "ix_alerts_type",
                table: "alerts",
                column: "AlertType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");
        }
    }
}
