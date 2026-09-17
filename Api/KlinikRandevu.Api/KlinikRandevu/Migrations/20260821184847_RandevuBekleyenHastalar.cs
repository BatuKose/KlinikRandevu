using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class RandevuBekleyenHastalar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RandevuBekleyenHastalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tcKimlik = table.Column<long>(type: "bigint", nullable: false),
                    protokol = table.Column<int>(type: "int", nullable: false),
                    doktorNo = table.Column<int>(type: "int", nullable: false),
                    polNo = table.Column<int>(type: "int", nullable: false),
                    RandevuTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    Bilgilendirme = table.Column<bool>(type: "bit", nullable: false),
                    RandevuVerildi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RandevuBekleyenHastalar", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 21, 21, 48, 46, 584, DateTimeKind.Local).AddTicks(9247));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 21, 21, 48, 46, 587, DateTimeKind.Local).AddTicks(1115));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 21, 21, 48, 46, 587, DateTimeKind.Local).AddTicks(1131));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RandevuBekleyenHastalar");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 15, 15, 23, 16, 700, DateTimeKind.Local).AddTicks(8681));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 15, 15, 23, 16, 701, DateTimeKind.Local).AddTicks(9843));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 15, 15, 23, 16, 701, DateTimeKind.Local).AddTicks(9853));
        }
    }
}
