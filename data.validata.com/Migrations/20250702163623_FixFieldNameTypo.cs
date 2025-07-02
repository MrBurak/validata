using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.validata.com.Migrations
{
    /// <inheritdoc />
    public partial class FixFieldNameTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Addess",
                schema: "Validata",
                table: "Customer",
                newName: "Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                schema: "Validata",
                table: "Customer",
                newName: "Addess");
        }
    }
}
