using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorIdToMentions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ca_mention_records_tweet_ca_unique",
                table: "ca_mention_records");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "ca_mention_records",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_ca_mention_records_tweet_ca_author_unique",
                table: "ca_mention_records",
                columns: new[] { "TweetId", "CaAddress", "AuthorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ca_mention_records_tweet_ca_author_unique",
                table: "ca_mention_records");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "ca_mention_records");

            migrationBuilder.CreateIndex(
                name: "ix_ca_mention_records_tweet_ca_unique",
                table: "ca_mention_records",
                columns: new[] { "TweetId", "CaAddress" },
                unique: true);
        }
    }
}
