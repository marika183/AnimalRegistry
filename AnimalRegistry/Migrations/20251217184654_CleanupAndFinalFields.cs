using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalRegistry.Migrations
{
    /// <inheritdoc />
    public partial class CleanupAndFinalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Breed",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Animals",
                newName: "CountryOfOrigin");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Animals",
                newName: "Source");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Animals",
                newName: "Purpose");

            migrationBuilder.AlterColumn<string>(
                name: "IdentificationNumber",
                table: "Animals",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Source",
                table: "Animals",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "Purpose",
                table: "Animals",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "CountryOfOrigin",
                table: "Animals",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "IdentificationNumber",
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
        }
    }
}
