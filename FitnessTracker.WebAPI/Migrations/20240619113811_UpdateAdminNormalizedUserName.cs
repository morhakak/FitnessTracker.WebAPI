using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminNormalizedUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bfbcaff4-57dc-4568-82c7-5d9437c1c3ee",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "NormalizedUserName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b93745a0-d85f-4d47-a186-09d1ddc2be65", "6/19/2024 2:38:11 PM", "ADMIN", "AQAAAAIAAYagAAAAEA1xXRpb6j5rFWChRPLo5bQ6VXFET+7sMRDrXRcXv1l08YtQcbfCyzZxubfFBgtznA==", "a150a8d9-ef22-4680-8237-a94b99dd9dd4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bfbcaff4-57dc-4568-82c7-5d9437c1c3ee",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "NormalizedUserName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2451f71-f3c1-4c5a-9686-a4465221178a", "6/19/2024 2:30:52 PM", "ADMIN@ADMIN.COM", "AQAAAAIAAYagAAAAEIWXp/M3G38zgqMVLzE1XRjiDv9u8SqebHuDQNajDoVRRIUnv4O8fpDtgG1E4WgjyA==", "6750e3b1-cb85-43d5-baeb-798d53188e21" });
        }
    }
}
