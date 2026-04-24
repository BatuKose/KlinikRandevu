using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class PatientV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Gender", "IsActive" },
                values: new object[] { new DateTime(2026, 4, 24, 22, 48, 42, 207, DateTimeKind.Local).AddTicks(8294), "M", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Gender", "IsActive" },
                values: new object[] { new DateTime(2026, 4, 24, 22, 45, 41, 77, DateTimeKind.Local).AddTicks(635), "\0", false });
        }
    }
}
