using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class parametrelerSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 21, 24, 17, 493, DateTimeKind.Local).AddTicks(3801));

            migrationBuilder.InsertData(
                table: "parametreler",
                columns: new[] { "Id", "Aciklama", "Aktif", "Deger1", "Deger2", "Deger3", "Deger4", "Deger5", "GuncellemeTarihi", "OlusturmaTarihi", "ParametreAdi" },
                values: new object[,]
                {
                    { 1, "D1: Aktif mi (EVET/HAYIR), D2: Hata mesajı", false, "EVET", null, null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "KADIN_DOGUM_ERKEK_YASAKLA" },
                    { 2, "D1: Aktif mi, D2: Min yaş, D3: Max yaş", false, "EVET", "0", "16", null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PEDIATRI_YAS_LIMITI" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 11, 21, 18, 49, 263, DateTimeKind.Local).AddTicks(7738));
        }
    }
}
