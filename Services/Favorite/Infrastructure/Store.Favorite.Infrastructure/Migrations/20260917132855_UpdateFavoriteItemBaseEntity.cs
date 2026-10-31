using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Store.Favorite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFavoriteItemBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteItems",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "FavoriteItemId",
                table: "FavoriteItems");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "FavoriteItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeleteDate",
                table: "FavoriteItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleteUserId",
                table: "FavoriteItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "InsertDate",
                table: "FavoriteItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "InsertUserId",
                table: "FavoriteItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FavoriteItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FavoriteItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "FavoriteItems",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdateDate",
                table: "FavoriteItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdateUserId",
                table: "FavoriteItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteItems",
                table: "FavoriteItems",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteItems",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "DeleteUserId",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "InsertUserId",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "UpdateUserId",
                table: "FavoriteItems");

            migrationBuilder.AddColumn<int>(
                name: "FavoriteItemId",
                table: "FavoriteItems",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteItems",
                table: "FavoriteItems",
                column: "FavoriteItemId");
        }
    }
}
