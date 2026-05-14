using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class patientEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Email" },
                values: new object[] { new DateTime(2026, 5, 14, 18, 38, 31, 648, DateTimeKind.Local).AddTicks(5022), null });

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Aktif", "OlusturmaTarihi" },
                values: new object[] { true, new DateTime(2026, 5, 14, 18, 38, 31, 649, DateTimeKind.Local).AddTicks(3558) });

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Aktif", "OlusturmaTarihi" },
                values: new object[] { true, new DateTime(2026, 5, 14, 18, 38, 31, 649, DateTimeKind.Local).AddTicks(3564) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Patients");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 21, 24, 17, 493, DateTimeKind.Local).AddTicks(3801));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Aktif", "OlusturmaTarihi" },
                values: new object[] { false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Aktif", "OlusturmaTarihi" },
                values: new object[] { false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
