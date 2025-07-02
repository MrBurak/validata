using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace data.validata.com.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Validata");

            migrationBuilder.CreateTable(
                name: "OperationSource",
                schema: "Validata",
                columns: table => new
                {
                    OperationSourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationSource", x => x.OperationSourceId);
                });

            migrationBuilder.InsertData(
                schema: "Validata",
                table: "OperationSource",
                columns: new[] { "OperationSourceId", "Name" },
                values: new object[,]
                {
                    { 1, "Pre Defined" },
                    { 2, "Api" },
                    { 3, "Import" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationSource",
                schema: "Validata");
        }
    }
}
