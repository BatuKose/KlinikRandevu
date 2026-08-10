using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class Tahütname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 9, 14, 36, 19, 517, DateTimeKind.Local).AddTicks(3823));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 9, 14, 36, 19, 518, DateTimeKind.Local).AddTicks(4323));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 9, 14, 36, 19, 518, DateTimeKind.Local).AddTicks(4330));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 8, 22, 40, 10, 436, DateTimeKind.Local).AddTicks(8845));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 8, 22, 40, 10, 437, DateTimeKind.Local).AddTicks(9965));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 8, 22, 40, 10, 437, DateTimeKind.Local).AddTicks(9973));
        }
    }
}
