using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class TedaviKaydi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TedaviKaydi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MuyaneId = table.Column<int>(type: "int", nullable: false),
                    doktorId = table.Column<int>(type: "int", nullable: false),
                    fiyat = table.Column<double>(type: "float", nullable: false),
                    tedaviKodu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tedaviAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Odendi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TedaviKaydi", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 8, 22, 0, 1, 614, DateTimeKind.Local).AddTicks(9745));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 8, 22, 0, 1, 616, DateTimeKind.Local).AddTicks(1054));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 8, 22, 0, 1, 616, DateTimeKind.Local).AddTicks(1062));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TedaviKaydi");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 15, 13, 52, 28, 606, DateTimeKind.Local).AddTicks(2645));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 7, 15, 13, 52, 28, 607, DateTimeKind.Local).AddTicks(3156));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 7, 15, 13, 52, 28, 607, DateTimeKind.Local).AddTicks(3167));
        }
    }
}
