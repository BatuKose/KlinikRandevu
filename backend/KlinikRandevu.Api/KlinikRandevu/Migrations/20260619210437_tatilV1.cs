using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class tatilV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tatil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TatilAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime", nullable: false),
                    MyProperty = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tatil", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 20, 0, 4, 36, 921, DateTimeKind.Local).AddTicks(1703));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 6, 20, 0, 4, 36, 922, DateTimeKind.Local).AddTicks(5930));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 6, 20, 0, 4, 36, 922, DateTimeKind.Local).AddTicks(5946));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tatil");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 20, 0, 3, 53, 555, DateTimeKind.Local).AddTicks(2216));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 6, 20, 0, 3, 53, 556, DateTimeKind.Local).AddTicks(1942));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 6, 20, 0, 3, 53, 556, DateTimeKind.Local).AddTicks(1950));
        }
    }
}
