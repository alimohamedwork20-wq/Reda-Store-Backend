using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reda.Migrations
{
    /// <inheritdoc />
    public partial class editReportTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Reports",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Reports",
                newName: "Screenshot");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Reports",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Screenshot",
                table: "Reports",
                newName: "Email");
        }
    }
}
