using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.validata.com.Migrations
{
    /// <inheritdoc />
    public partial class FixDataStructurePerformanceFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductCount",
                schema: "Validata",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "TotalAmount",
                schema: "Validata",
                table: "Order",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCount",
                schema: "Validata",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                schema: "Validata",
                table: "Order");
        }
    }
}
