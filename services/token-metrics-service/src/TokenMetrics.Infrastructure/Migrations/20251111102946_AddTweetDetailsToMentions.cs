using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTweetDetailsToMentions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOriginalPost",
                table: "ca_mention_records",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TweetContent",
                table: "ca_mention_records",
                type: "character varying(3000)",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TweetUrl",
                table: "ca_mention_records",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOriginalPost",
                table: "ca_mention_records");

            migrationBuilder.DropColumn(
                name: "TweetContent",
                table: "ca_mention_records");

            migrationBuilder.DropColumn(
                name: "TweetUrl",
                table: "ca_mention_records");
        }
    }
}
