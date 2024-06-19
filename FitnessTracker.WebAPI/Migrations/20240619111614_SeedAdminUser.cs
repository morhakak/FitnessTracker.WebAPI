using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "ImageUrl", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "bfbcaff4-57dc-4568-82c7-5d9437c1c3ee", 0, "8d16e4d4-984b-456d-adf8-cc8fee2a0b70", "", "admin@admin.com", true, "", false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAIAAYagAAAAEGyu8UzgLZ40ttyiQ4p4oTzAiUzE1crwpNRD0rAiRjXKliu+J6O8RYHwMmfbC8C1Kg==", null, false, "", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "661420f9-2a30-40cf-afa0-edc498392318", "bfbcaff4-57dc-4568-82c7-5d9437c1c3ee" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "661420f9-2a30-40cf-afa0-edc498392318", "bfbcaff4-57dc-4568-82c7-5d9437c1c3ee" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bfbcaff4-57dc-4568-82c7-5d9437c1c3ee");
        }
    }
}
