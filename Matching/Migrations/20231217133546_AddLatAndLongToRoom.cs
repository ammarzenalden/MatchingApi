using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matching.Migrations
{
    /// <inheritdoc />
    public partial class AddLatAndLongToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lat",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Long",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Long",
                table: "Rooms");
        }
    }
}
