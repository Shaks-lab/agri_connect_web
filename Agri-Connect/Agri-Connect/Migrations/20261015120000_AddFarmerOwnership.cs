using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agri_Connect.Migrations
{
    /// <inheritdoc />
    public partial class AddFarmerOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Products created before ownership existed have no farmer to belong to,
            // so they are removed before the required FarmerId column is added.
            migrationBuilder.Sql("DELETE FROM [Products]");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Farmers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FarmerId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Farmers_UserId",
                table: "Farmers",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_FarmerId",
                table: "Products",
                column: "FarmerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Farmers_AspNetUsers_UserId",
                table: "Farmers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Farmers_FarmerId",
                table: "Products",
                column: "FarmerId",
                principalTable: "Farmers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farmers_AspNetUsers_UserId",
                table: "Farmers");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Farmers_FarmerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Farmers_UserId",
                table: "Farmers");

            migrationBuilder.DropIndex(
                name: "IX_Products_FarmerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Farmers");

            migrationBuilder.DropColumn(
                name: "FarmerId",
                table: "Products");
        }
    }
}
