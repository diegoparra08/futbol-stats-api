using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolStatsWithFriends.Migrations
{
    /// <inheritdoc />
    public partial class AddTacticalPositionIndexToMatchDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TacticalPositionIndex",
                table: "MatchDetails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TacticalPositionIndex",
                table: "MatchDetails");
        }
    }
}
