using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tally.Hooks.Migrations
{
    /// <inheritdoc />
    public partial class RenamedLearningHoursProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LearingHours",
                table: "Certificates",
                newName: "LearningHours");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LearningHours",
                table: "Certificates",
                newName: "LearingHours");
        }
    }
}
