using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureToMentions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "ca_mention_records",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "ca_mention_records");
        }
    }
}
