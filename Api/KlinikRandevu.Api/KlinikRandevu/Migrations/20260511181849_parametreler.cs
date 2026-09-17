using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class parametreler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "parametreler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParametreAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Deger1 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Deger2 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Deger3 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Deger4 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Deger5 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parametreler", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 21, 18, 49, 263, DateTimeKind.Local).AddTicks(7738));

            migrationBuilder.CreateIndex(
                name: "IX_parametreler_ParametreAdi",
                table: "parametreler",
                column: "ParametreAdi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parametreler");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 19, 34, 22, 844, DateTimeKind.Local).AddTicks(5178));
        }
    }
}
