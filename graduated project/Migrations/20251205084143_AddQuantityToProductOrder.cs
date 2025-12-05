using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace graduated_project.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityToProductOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductOrders");
        }
    }
}
