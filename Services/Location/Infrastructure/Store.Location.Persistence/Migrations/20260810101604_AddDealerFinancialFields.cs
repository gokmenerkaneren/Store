using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Location.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDealerFinancialFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CreditLimit",
                table: "Dealers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBalance",
                table: "Dealers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountRate",
                table: "Dealers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "Dealers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaxOffice",
                table: "Dealers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditLimit",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "CurrentBalance",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "DiscountRate",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "TaxOffice",
                table: "Dealers");
        }
    }
}
