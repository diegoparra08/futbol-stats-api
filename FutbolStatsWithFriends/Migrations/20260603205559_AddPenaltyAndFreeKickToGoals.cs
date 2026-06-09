using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolStatsWithFriends.Migrations
{
    /// <inheritdoc />
    public partial class AddPenaltyAndFreeKickToGoals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FoulsCommitted",
                table: "MatchDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Recoveries",
                table: "MatchDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Tackles",
                table: "MatchDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssistedByPlayerId",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeKick",
                table: "Goals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPenalty",
                table: "Goals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Goals_AssistedByPlayerId",
                table: "Goals",
                column: "AssistedByPlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Players_AssistedByPlayerId",
                table: "Goals",
                column: "AssistedByPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Players_AssistedByPlayerId",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_Goals_AssistedByPlayerId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "FoulsCommitted",
                table: "MatchDetails");

            migrationBuilder.DropColumn(
                name: "Recoveries",
                table: "MatchDetails");

            migrationBuilder.DropColumn(
                name: "Tackles",
                table: "MatchDetails");

            migrationBuilder.DropColumn(
                name: "AssistedByPlayerId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "IsFreeKick",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "IsPenalty",
                table: "Goals");
        }
    }
}
