using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class Taahütname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "taahütname",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MuayeneId = table.Column<int>(type: "int", nullable: false),
                    ToplamBorc = table.Column<double>(type: "float", nullable: false),
                    TahütTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    SonOdemeTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    BilgilendirmeSms = table.Column<bool>(type: "bit", nullable: false),
                    BilgilendirmeMail = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taahütname", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 9, 14, 37, 44, 363, DateTimeKind.Local).AddTicks(958));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 9, 14, 37, 44, 364, DateTimeKind.Local).AddTicks(1203));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 8, 9, 14, 37, 44, 364, DateTimeKind.Local).AddTicks(1209));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "taahütname");

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
    }
}
