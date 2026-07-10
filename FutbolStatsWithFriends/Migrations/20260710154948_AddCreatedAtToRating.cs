using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolStatsWithFriends.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Ratings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Ratings");
        }
    }
}
