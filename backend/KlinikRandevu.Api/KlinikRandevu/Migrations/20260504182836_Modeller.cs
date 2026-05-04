using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class Modeller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalismaPlanlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoktorNo = table.Column<int>(type: "int", nullable: false),
                    PolNo = table.Column<int>(type: "int", nullable: false),
                    GunAdi = table.Column<int>(type: "int", nullable: false),
                    BaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    RandevuSuresiDk = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalismaPlanlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Doktorlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    doktorNo = table.Column<int>(type: "int", nullable: false),
                    DoktorAd = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    doktorUzKod = table.Column<int>(type: "int", nullable: false),
                    doktorTc = table.Column<long>(type: "bigint", nullable: false),
                    ServisNo = table.Column<int>(type: "int", nullable: false),
                    tescilNO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doktorlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MuayeneKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProtocolNo = table.Column<int>(type: "int", nullable: false),
                    DoktorNo = table.Column<int>(type: "int", nullable: false),
                    PolNo = table.Column<int>(type: "int", nullable: false),
                    HastaTc = table.Column<long>(type: "bigint", nullable: false),
                    MuayeneTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaslangicSaati = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisSaati = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RandevuId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuayeneKayitlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Randevular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoktorNo = table.Column<int>(type: "int", nullable: false),
                    PolNo = table.Column<int>(type: "int", nullable: false),
                    HastaTc = table.Column<long>(type: "bigint", nullable: false),
                    ProtocolNo = table.Column<int>(type: "int", nullable: false),
                    RandevuTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SureDakika = table.Column<int>(type: "int", nullable: false),
                    Notlar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Randevular", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CalismaPlanlari",
                columns: new[] { "Id", "BaslangicSaati", "BitisSaati", "DoktorNo", "GunAdi", "IsActive", "PolNo", "RandevuSuresiDk" },
                values: new object[,]
                {
                    { 1, new TimeSpan(0, 8, 0, 0, 0), new TimeSpan(0, 12, 0, 0, 0), 1, 1, true, 1, 10 },
                    { 2, new TimeSpan(0, 8, 0, 0, 0), new TimeSpan(0, 12, 0, 0, 0), 1, 3, true, 1, 10 },
                    { 3, new TimeSpan(0, 8, 0, 0, 0), new TimeSpan(0, 12, 0, 0, 0), 1, 5, true, 1, 10 },
                    { 4, new TimeSpan(0, 9, 0, 0, 0), new TimeSpan(0, 17, 0, 0, 0), 2, 2, true, 2, 15 },
                    { 5, new TimeSpan(0, 9, 0, 0, 0), new TimeSpan(0, 17, 0, 0, 0), 2, 4, true, 2, 15 },
                    { 6, new TimeSpan(0, 10, 0, 0, 0), new TimeSpan(0, 16, 0, 0, 0), 3, 1, true, 3, 20 },
                    { 7, new TimeSpan(0, 10, 0, 0, 0), new TimeSpan(0, 16, 0, 0, 0), 3, 2, true, 3, 20 },
                    { 8, new TimeSpan(0, 10, 0, 0, 0), new TimeSpan(0, 16, 0, 0, 0), 3, 3, true, 3, 20 },
                    { 9, new TimeSpan(0, 10, 0, 0, 0), new TimeSpan(0, 16, 0, 0, 0), 3, 4, true, 3, 20 },
                    { 10, new TimeSpan(0, 10, 0, 0, 0), new TimeSpan(0, 16, 0, 0, 0), 3, 5, true, 3, 20 }
                });

            migrationBuilder.InsertData(
                table: "Doktorlar",
                columns: new[] { "Id", "DoktorAd", "ServisNo", "doktorNo", "doktorTc", "doktorUzKod", "tescilNO" },
                values: new object[,]
                {
                    { 1, "HÜSEYİN AĞAC", 1, 1, 11111111111L, 1000, 10001 },
                    { 2, "AYŞE DEMİR", 2, 2, 22222222222L, 1100, 10002 },
                    { 3, "MEHMET YILMAZ", 3, 3, 33333333333L, 1300, 10003 }
                });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "TcKimlik" },
                values: new object[] { new DateTime(2026, 5, 4, 21, 28, 35, 691, DateTimeKind.Local).AddTicks(44), 11111111111L });

            migrationBuilder.CreateIndex(
                name: "IX_Doktorlar_doktorNo",
                table: "Doktorlar",
                column: "doktorNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doktorlar_doktorTc",
                table: "Doktorlar",
                column: "doktorTc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_DoktorNo_RandevuTarihi",
                table: "Randevular",
                columns: new[] { "DoktorNo", "RandevuTarihi" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalismaPlanlari");

            migrationBuilder.DropTable(
                name: "Doktorlar");

            migrationBuilder.DropTable(
                name: "MuayeneKayitlari");

            migrationBuilder.DropTable(
                name: "Randevular");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "TcKimlik" },
                values: new object[] { new DateTime(2026, 5, 2, 21, 58, 6, 639, DateTimeKind.Local).AddTicks(4840), 0L });
        }
    }
}
