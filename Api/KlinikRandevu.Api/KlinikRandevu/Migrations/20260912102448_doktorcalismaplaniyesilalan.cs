using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class doktorcalismaplaniyesilalan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "YesilAlanZorunlu",
                table: "CalismaPlanlari",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "CalismaPlanlari",
                keyColumn: "Id",
                keyValue: 1,
                column: "YesilAlanZorunlu",
                value: false);

            migrationBuilder.UpdateData(
                table: "CalismaPlanlari",
                keyColumn: "Id",
                keyValue: 2,
                column: "YesilAlanZorunlu",
                value: false);

            migrationBuilder.UpdateData(
                table: "CalismaPlanlari",
                keyColumn: "Id",
                keyValue: 3,
                column: "YesilAlanZorunlu",
                value: false);

            migrationBuilder.UpdateData(
                table: "CalismaPlanlari",
                keyColumn: "Id",
                keyValue: 4,
                column: "YesilAlanZorunlu",
                value: false);

            migrationBuilder.UpdateData(
                table: "CalismaPlanlari",
                keyColumn: "Id",
                keyValue: 5,
                column: "YesilAlanZorunlu",
                value: false);

            migrationBuilder.UpdateData(
                table: "CalismaPlanlari",
                keyColumn: "Id",
                keyValue: 6,
                column: "YesilAlanZorunlu",
                value: false);

            migrationBuilder.UpdateData(
                table: "CalismaPlanlari",
                keyColumn: "Id",
                keyValue: 7,
                column: "YesilAlanZorunlu",
                value: false);

            migrationBuilder.UpdateData(
                table: "CalismaPlanlari",
                keyColumn: "Id",
                keyValue: 8,
                column: "YesilAlanZorunlu",
                value: false);

            migrationBuilder.UpdateData(
                table: "CalismaPlanlari",
                keyColumn: "Id",
                keyValue: 9,
                column: "YesilAlanZorunlu",
                value: false);

            migrationBuilder.UpdateData(
                table: "CalismaPlanlari",
                keyColumn: "Id",
                keyValue: 10,
                column: "YesilAlanZorunlu",
                value: false);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 9, 12, 13, 24, 47, 889, DateTimeKind.Local).AddTicks(2853));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 9, 12, 13, 24, 47, 890, DateTimeKind.Local).AddTicks(1540));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 9, 12, 13, 24, 47, 890, DateTimeKind.Local).AddTicks(1546));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YesilAlanZorunlu",
                table: "CalismaPlanlari");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 9, 11, 18, 32, 24, 936, DateTimeKind.Local).AddTicks(9731));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 9, 11, 18, 32, 24, 937, DateTimeKind.Local).AddTicks(8354));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 9, 11, 18, 32, 24, 937, DateTimeKind.Local).AddTicks(8361));
        }
    }
}
