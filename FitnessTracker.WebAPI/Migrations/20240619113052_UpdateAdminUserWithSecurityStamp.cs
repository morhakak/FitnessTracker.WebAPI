using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminUserWithSecurityStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bfbcaff4-57dc-4568-82c7-5d9437c1c3ee",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2451f71-f3c1-4c5a-9686-a4465221178a", "6/19/2024 2:30:52 PM", "AQAAAAIAAYagAAAAEIWXp/M3G38zgqMVLzE1XRjiDv9u8SqebHuDQNajDoVRRIUnv4O8fpDtgG1E4WgjyA==", "6750e3b1-cb85-43d5-baeb-798d53188e21" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bfbcaff4-57dc-4568-82c7-5d9437c1c3ee",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6918a659-0ddd-4237-b0c8-90ebb3c20a08", "6/19/2024 2:21:00 PM", "AQAAAAIAAYagAAAAENz5osKXwdlT2lRApBVTOIXPzyoC4k7Um1F/zBLIH60eUJ2tnkh2eW9LxoV3qpvP0Q==", "" });
        }
    }
}
