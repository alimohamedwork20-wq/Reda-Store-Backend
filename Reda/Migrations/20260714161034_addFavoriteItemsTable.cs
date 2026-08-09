using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reda.Migrations
{
    /// <inheritdoc />
    public partial class addFavoriteItemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Products_ProductId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_ProductId",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Favorites");

            migrationBuilder.RenameColumn(
                name: "AddedAt",
                table: "Favorites",
                newName: "CreatedAt");

            migrationBuilder.CreateTable(
                name: "FavoriteItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FavoriteId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteItems_Favorites_FavoriteId",
                        column: x => x.FavoriteId,
                        principalTable: "Favorites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItems_FavoriteId",
                table: "FavoriteItems",
                column: "FavoriteId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItems_ProductId",
                table: "FavoriteItems",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteItems");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Favorites",
                newName: "AddedAt");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Favorites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ProductId",
                table: "Favorites",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Products_ProductId",
                table: "Favorites",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
