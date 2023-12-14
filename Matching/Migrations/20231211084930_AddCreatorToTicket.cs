using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matching.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatorId",
                table: "Tickets",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_CreatorId",
                table: "Tickets",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_CreatorId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CreatorId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Tickets");
        }
    }
}
