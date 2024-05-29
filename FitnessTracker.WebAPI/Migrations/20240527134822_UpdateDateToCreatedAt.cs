using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDateToCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Workouts",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Workouts",
                newName: "Date");
        }
    }
}
