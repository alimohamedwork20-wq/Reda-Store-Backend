using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reda.Migrations
{
    /// <inheritdoc />
    public partial class refresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Detailed",
                table: "Addresses",
                newName: "Details");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Details",
                table: "Addresses",
                newName: "Detailed");
        }
    }
}
