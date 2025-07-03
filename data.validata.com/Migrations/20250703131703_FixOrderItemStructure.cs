using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.validata.com.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderItemStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ProductPrice",
                schema: "Validata",
                table: "OrderItem",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductPrice",
                schema: "Validata",
                table: "OrderItem");
        }
    }
}
