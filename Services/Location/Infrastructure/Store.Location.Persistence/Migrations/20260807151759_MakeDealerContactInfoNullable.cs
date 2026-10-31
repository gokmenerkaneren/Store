using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Location.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeDealerContactInfoNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dealers_ContactInfos_ContactInfoId",
                table: "Dealers");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactInfoId",
                table: "Dealers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Dealers_ContactInfos_ContactInfoId",
                table: "Dealers",
                column: "ContactInfoId",
                principalTable: "ContactInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dealers_ContactInfos_ContactInfoId",
                table: "Dealers");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactInfoId",
                table: "Dealers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dealers_ContactInfos_ContactInfoId",
                table: "Dealers",
                column: "ContactInfoId",
                principalTable: "ContactInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
