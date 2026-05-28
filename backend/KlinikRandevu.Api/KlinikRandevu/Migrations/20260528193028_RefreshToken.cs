using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "users",
                type: "datetime",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 22, 30, 27, 151, DateTimeKind.Local).AddTicks(9338));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 5, 28, 22, 30, 27, 153, DateTimeKind.Local).AddTicks(5471));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 5, 28, 22, 30, 27, 153, DateTimeKind.Local).AddTicks(5493));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 1,
                columns: new[] { "RefreshToken", "RefreshTokenExpiry" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "users");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 27, 18, 36, 59, 637, DateTimeKind.Local).AddTicks(8636));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 5, 27, 18, 36, 59, 639, DateTimeKind.Local).AddTicks(4686));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 5, 27, 18, 36, 59, 639, DateTimeKind.Local).AddTicks(4702));
        }
    }
}
