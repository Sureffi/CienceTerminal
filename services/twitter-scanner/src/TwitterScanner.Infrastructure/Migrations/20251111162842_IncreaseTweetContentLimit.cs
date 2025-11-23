using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterScanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseTweetContentLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alter existing column to increase max length from 3000 to 30000
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
            // Revert column back to original max length
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
