using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shipkart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSchemaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryEstimate",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                table: "Products",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeliveryEstimate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Manufacturer",
                table: "Products");
        }
    }
}
