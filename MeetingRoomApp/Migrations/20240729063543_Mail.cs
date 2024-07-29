using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingRoomApp.Migrations
{
    /// <inheritdoc />
    public partial class Mail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReminderSent",
                table: "Meetings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "MeetingParticipants",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderSent",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "MeetingParticipants");
        }
    }
}
