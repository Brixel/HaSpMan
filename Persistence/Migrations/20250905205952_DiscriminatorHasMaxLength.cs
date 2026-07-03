using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DiscriminatorHasMaxLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP VIEW IF EXISTS HaSpMan.vwBankAccountTotals");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                schema: "HaSpMan",
                table: "Transactions",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.Sql($@"CREATE OR ALTER VIEW HaSpMan.vwBankAccountTotals
                                        WITH SCHEMABINDING
                                        AS
                                            SELECT
                                                SUM(
                                                    CASE t.Discriminator
                                                        WHEN 'CreditTransaction' THEN t.Amount * -1
                                                        ELSE t.Amount
                                                    END
                                                ) AS Amount,
                                                t.BankAccountId,
                                                COUNT_BIG(*) AS NumberOfTransactions
                                            FROM
                                                HaSpMan.Transactions t
                                                GROUP BY t.BankAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                schema: "HaSpMan",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);
        }
    }
}
