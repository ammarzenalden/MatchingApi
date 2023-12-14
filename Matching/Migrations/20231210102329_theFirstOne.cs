using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matching.Migrations
{
    /// <inheritdoc />
    public partial class theFirstOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonalPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SexualPreference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommitmentLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalBelieves = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FavoriteHolidayDestination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FreeTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MusicGenres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Languages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Height = table.Column<float>(type: "real", nullable: false),
                    BodyType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PotentialPartnerPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinAge = table.Column<int>(type: "int", nullable: false),
                    MaxAge = table.Column<int>(type: "int", nullable: false),
                    BodyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommitmentLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalBelieves = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FavoriteHolidayDestination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FreeTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MusicGenres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PotentialPartnerPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PotentialPartnerPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User1 = table.Column<int>(type: "int", nullable: true),
                    User2 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Users_User1",
                        column: x => x.User1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Rooms_Users_User2",
                        column: x => x.User2,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonalPreferences_UserId",
                table: "PersonalPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PotentialPartnerPreferences_UserId",
                table: "PotentialPartnerPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_User1",
                table: "Rooms",
                column: "User1");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_User2",
                table: "Rooms",
                column: "User2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonalPreferences");

            migrationBuilder.DropTable(
                name: "PotentialPartnerPreferences");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
