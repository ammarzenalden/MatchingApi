using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matching.Migrations
{
    /// <inheritdoc />
    public partial class AddRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_User1",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_User2",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_User1",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "User1",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "TicketType",
                table: "UserTickets",
                newName: "TicketStatus");

            migrationBuilder.RenameColumn(
                name: "User2",
                table: "Rooms",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_User2",
                table: "Rooms",
                newName: "IX_Rooms_CreatorId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requests_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ReceiverId",
                table: "Requests",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SenderId",
                table: "Requests",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_CreatorId",
                table: "Rooms",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_CreatorId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "TicketStatus",
                table: "UserTickets",
                newName: "TicketType");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Rooms",
                newName: "User2");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_CreatorId",
                table: "Rooms",
                newName: "IX_Rooms_User2");

            migrationBuilder.AddColumn<int>(
                name: "User1",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_User1",
                table: "Rooms",
                column: "User1");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_User1",
                table: "Rooms",
                column: "User1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_User2",
                table: "Rooms",
                column: "User2",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
