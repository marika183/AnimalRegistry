using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalRegistry.Migrations
{
    /// <inheritdoc />
    public partial class FinalDatabaseUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AspNetUsers_OwnerId",
                table: "Animals");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Animals",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Breed",
                table: "Animals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfDeath",
                table: "Animals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Animals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Animals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumber",
                table: "Animals",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AspNetUsers_OwnerId",
                table: "Animals",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AspNetUsers_OwnerId",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Breed",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "DateOfDeath",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "IdentificationNumber",
                table: "Animals");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Animals",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AspNetUsers_OwnerId",
                table: "Animals",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
