using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MeetingRoomApp.Migrations
{
    /// <inheritdoc />
    public partial class IdForMParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MeetingParticipants",
                table: "MeetingParticipants");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MeetingParticipants",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeetingParticipants",
                table: "MeetingParticipants",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingParticipants_MeetingId",
                table: "MeetingParticipants",
                column: "MeetingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MeetingParticipants",
                table: "MeetingParticipants");

            migrationBuilder.DropIndex(
                name: "IX_MeetingParticipants_MeetingId",
                table: "MeetingParticipants");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MeetingParticipants");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeetingParticipants",
                table: "MeetingParticipants",
                columns: new[] { "MeetingId", "UserId" });
        }
    }
}
