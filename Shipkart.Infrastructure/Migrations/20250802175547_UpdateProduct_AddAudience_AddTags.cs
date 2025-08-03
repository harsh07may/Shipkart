using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shipkart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProduct_AddAudience_AddTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryEstimate",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Audience",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Audience",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryEstimate",
                table: "Products",
                type: "text",
                nullable: true);
        }
    }
}
