using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nimble.Modulith.Products.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductsForLab6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                schema: "Products",
                table: "Products",
                type: "int",
                maxLength: 200,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                schema: "Products",
                table: "Products");
        }
    }
}
