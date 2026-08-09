using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reda.Migrations
{
    /// <inheritdoc />
    public partial class addTwoFactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TwoFactor",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactor",
                table: "Users");
        }
    }
}
