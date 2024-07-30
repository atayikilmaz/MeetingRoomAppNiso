using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingRoomApp.Migrations
{
    /// <inheritdoc />
    public partial class UniqueMeetingRoomName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MeetingRooms_Name",
                table: "MeetingRooms",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MeetingRooms_Name",
                table: "MeetingRooms");
        }
    }
}
