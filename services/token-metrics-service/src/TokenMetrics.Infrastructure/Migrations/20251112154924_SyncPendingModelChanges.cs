using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenMetrics.Infrastructure.Migrations
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

            migrationBuilder.AlterColumn<string>(
                name: "TweetContent",
                table: "ca_mention_records",
                type: "character varying(30000)",
                maxLength: 30000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(3000)",
                oldMaxLength: 3000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoinMintAddress",
                table: "ca_mention_records",
                newName: "CaAddress");

            migrationBuilder.AlterColumn<string>(
                name: "TweetContent",
                table: "ca_mention_records",
                type: "character varying(3000)",
                maxLength: 3000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30000)",
                oldMaxLength: 30000,
                oldNullable: true);
        }
    }
}
