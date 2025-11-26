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
            // Only rename if CaAddress exists (idempotent migration)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'ca_mention_records'
                        AND column_name = 'CaAddress'
                    ) THEN
                        ALTER TABLE ca_mention_records RENAME COLUMN ""CaAddress"" TO ""CoinMintAddress"";
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Only rename if CoinMintAddress exists (idempotent migration)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'ca_mention_records'
                        AND column_name = 'CoinMintAddress'
                    ) THEN
                        ALTER TABLE ca_mention_records RENAME COLUMN ""CoinMintAddress"" TO ""CaAddress"";
                    END IF;
                END $$;
            ");
        }
    }
}
