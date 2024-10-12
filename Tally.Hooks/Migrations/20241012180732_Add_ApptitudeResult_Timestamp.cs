using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tally.Hooks.Migrations
{
    /// <inheritdoc />
    public partial class Add_ApptitudeResult_Timestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Timestamp",
                table: "AptitudeResults",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "AptitudeResults");
        }
    }
}
