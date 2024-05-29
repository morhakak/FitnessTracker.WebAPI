using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitnessTracker.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "661420f9-2a30-40cf-afa0-edc498392318", "661420f9-2a30-40cf-afa0-edc498392318", "admin", "ADMIN" },
                    { "743f9ebe-0fd5-4382-bf56-2baa0c34e80a", "743f9ebe-0fd5-4382-bf56-2baa0c34e80a", "user", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "661420f9-2a30-40cf-afa0-edc498392318");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "743f9ebe-0fd5-4382-bf56-2baa0c34e80a");
        }
    }
}
