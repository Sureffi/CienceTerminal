using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterScanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CaAddress",
                table: "ca_mention_records",
                newName: "CoinMintAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoinMintAddress",
                table: "ca_mention_records",
                newName: "CaAddress");
        }
    }
}
