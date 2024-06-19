using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bfbcaff4-57dc-4568-82c7-5d9437c1c3ee",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash" },
                values: new object[] { "6918a659-0ddd-4237-b0c8-90ebb3c20a08", "6/19/2024 2:21:00 PM", "AQAAAAIAAYagAAAAENz5osKXwdlT2lRApBVTOIXPzyoC4k7Um1F/zBLIH60eUJ2tnkh2eW9LxoV3qpvP0Q==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bfbcaff4-57dc-4568-82c7-5d9437c1c3ee",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash" },
                values: new object[] { "8d16e4d4-984b-456d-adf8-cc8fee2a0b70", "", "AQAAAAIAAYagAAAAEGyu8UzgLZ40ttyiQ4p4oTzAiUzE1crwpNRD0rAiRjXKliu+J6O8RYHwMmfbC8C1Kg==" });
        }
    }
}
