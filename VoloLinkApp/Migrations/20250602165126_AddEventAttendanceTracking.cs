using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoloLinkApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEventAttendanceTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_CreatorId",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "Events",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EventAttendance",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendance", x => new { x.EventId, x.UserId });
                    table.ForeignKey(
                        name: "FK_EventAttendance_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventAttendance_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendance_UserId",
                table: "EventAttendance",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_CreatorId",
                table: "Events",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_CreatorId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventAttendance");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "Events",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_CreatorId",
                table: "Events",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
