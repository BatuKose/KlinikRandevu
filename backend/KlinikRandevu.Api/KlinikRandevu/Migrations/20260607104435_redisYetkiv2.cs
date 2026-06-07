using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class redisYetkiv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 7, 13, 44, 34, 983, DateTimeKind.Local).AddTicks(5287));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 6, 7, 13, 44, 34, 984, DateTimeKind.Local).AddTicks(6395));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 6, 7, 13, 44, 34, 984, DateTimeKind.Local).AddTicks(6402));

            migrationBuilder.InsertData(
                table: "yetkiler",
                columns: new[] { "Id", "Ad", "Kod" },
                values: new object[] { 3, "Redis Atma", "Redis_atma" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "yetkiler",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 7, 13, 22, 26, 534, DateTimeKind.Local).AddTicks(8788));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 6, 7, 13, 22, 26, 535, DateTimeKind.Local).AddTicks(8823));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 6, 7, 13, 22, 26, 535, DateTimeKind.Local).AddTicks(8830));
        }
    }
}
